using System.Text.Json;
using Amazon.Lambda.Core;
using FluentAssertions;
using Moq;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Domain.Services;
using VideoProcessor.Lambda;
using Xunit;

namespace VideoProcessor.Tests.Unit.InterfacesExternas.Lambda;

public class FunctionHandlerTests
{
    private static ChunkProcessorInput ValidInput() =>
        new(
            ContractVersion: "1.0",
            VideoId: "video-test",
            Chunk: new ChunkInfo("chunk-001", 0, 60, 5),
            Source: new SourceInfo("bucket-src", "key/video.mp4"),
            Output: new OutputConfig("bucket-out", "prefix/", FramesBucket: "bucket-out", FramesPrefix: "prefix/frames/"));

    private static Function BuildSuccessSut(out Mock<ILambdaLogger> loggerMock)
    {
        var framePaths = new List<string> { "/tmp/f1.jpg", "/tmp/f2.jpg" };
        var extractResult = new FrameExtractionResult(2, framePaths, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);
        storageMock
            .Setup(x => x.UploadFramesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["prefix/frames/f1.jpg", "prefix/frames/f2.jpg"]);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(extractResult);

        var useCase = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        loggerMock = new Mock<ILambdaLogger>(MockBehavior.Loose);
        return new Function(useCase);
    }

    [Fact]
    public async Task FunctionHandler_ValidInput_ReturnsSerializedJsonWithSucceededStatus()
    {
        // Arrange
        var sut = BuildSuccessSut(out var loggerMock);
        var context = Mock.Of<ILambdaContext>(ctx =>
            ctx.Logger == loggerMock.Object &&
            ctx.RemainingTime == TimeSpan.Zero);

        // Act
        var result = await sut.FunctionHandler(ValidInput(), context);

        // Assert
        result.Should().NotBeNullOrEmpty();
        var doc = JsonDocument.Parse(result);
        doc.RootElement.GetProperty("status").GetString().Should().Be("SUCCEEDED");
        doc.RootElement.GetProperty("framesCount").GetInt32().Should().Be(2);
        doc.RootElement.GetProperty("chunkId").GetString().Should().Be("chunk-001");
    }

    [Fact]
    public async Task FunctionHandler_WhenRemainingTimeIsGreaterThan30s_ExecutesAndReturnsSucceeded()
    {
        // Arrange — cobre o branch que cria CancellationTokenSource com timeout
        var sut = BuildSuccessSut(out var loggerMock);
        var context = Mock.Of<ILambdaContext>(ctx =>
            ctx.Logger == loggerMock.Object &&
            ctx.RemainingTime == TimeSpan.FromMinutes(5));

        // Act
        var result = await sut.FunctionHandler(ValidInput(), context);

        // Assert
        result.Should().NotBeNullOrEmpty();
        var doc = JsonDocument.Parse(result);
        doc.RootElement.GetProperty("status").GetString().Should().Be("SUCCEEDED");
    }

    [Fact]
    public async Task FunctionHandler_ValidInput_LogsInformationOnStart()
    {
        // Arrange
        var sut = BuildSuccessSut(out var loggerMock);
        var context = Mock.Of<ILambdaContext>(ctx =>
            ctx.Logger == loggerMock.Object &&
            ctx.RemainingTime == TimeSpan.Zero);

        // Act
        await sut.FunctionHandler(ValidInput(), context);

        // Assert — LogInformation chamado com VideoId e ChunkId no início e no fim
        loggerMock.Verify(
            x => x.LogInformation(It.Is<string>(msg => msg.Contains("Iniciando")), It.IsAny<object[]>()),
            Times.Once);
        loggerMock.Verify(
            x => x.LogInformation(It.Is<string>(msg => msg.Contains("concluído")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task FunctionHandler_WhenFfmpegPathEnvVarSetToNonExistentDir_StillProcessesSuccessfully()
    {
        // Arrange — cobre o branch !string.IsNullOrWhiteSpace(envPath) em TrySetFfmpegPathFromEnvOrKnownPaths
        var prev = Environment.GetEnvironmentVariable("FFMPEG_PATH");
        Environment.SetEnvironmentVariable("FFMPEG_PATH", "/non/existent/path/xyz-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            var sut = BuildSuccessSut(out var loggerMock);
            var context = Mock.Of<ILambdaContext>(ctx =>
                ctx.Logger == loggerMock.Object &&
                ctx.RemainingTime == TimeSpan.Zero);

            // Act
            var result = await sut.FunctionHandler(ValidInput(), context);

            // Assert — mesmo com FFMPEG_PATH inválido, o handler processa normalmente
            result.Should().Contain("SUCCEEDED");
        }
        finally
        {
            Environment.SetEnvironmentVariable("FFMPEG_PATH", prev);
        }
    }

    [Fact]
    public async Task FunctionHandler_WhenFfmpegBinariesExistInEnvPath_ConfiguresPathAndSucceeds()
    {
        // Arrange — cobre File.Exists path, FFmpeg.SetExecutablesPath e IsFfmpegConfigured retornando true
        var tempDir = Path.Combine(Path.GetTempPath(), "ffmpeg-test-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);

        var ffmpegName = OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg";
        var ffprobeName = OperatingSystem.IsWindows() ? "ffprobe.exe" : "ffprobe";
        await File.WriteAllTextAsync(Path.Combine(tempDir, ffmpegName), "fake-binary");
        await File.WriteAllTextAsync(Path.Combine(tempDir, ffprobeName), "fake-binary");

        var prev = Environment.GetEnvironmentVariable("FFMPEG_PATH");
        Environment.SetEnvironmentVariable("FFMPEG_PATH", tempDir);
        try
        {
            var sut = BuildSuccessSut(out var loggerMock);
            var context = Mock.Of<ILambdaContext>(ctx =>
                ctx.Logger == loggerMock.Object &&
                ctx.RemainingTime == TimeSpan.Zero);

            // Act
            var result = await sut.FunctionHandler(ValidInput(), context);

            // Assert — path configurado a partir de FFMPEG_PATH, processamento com sucesso
            result.Should().Contain("SUCCEEDED");
        }
        finally
        {
            Environment.SetEnvironmentVariable("FFMPEG_PATH", prev);
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task FunctionHandler_WhenFfmpegExistsButFfprobeDoesNot_ContinuesWithoutSettingPath()
    {
        // Cobre o branch: File.Exists(ffmpegName) = true E File.Exists(ffprobeName) = false
        // → condição && fica false → não configura o path, continua loop
        var tempDir = Path.Combine(Path.GetTempPath(), "ffmpeg-only-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);

        var ffmpegName = OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg";
        await File.WriteAllTextAsync(Path.Combine(tempDir, ffmpegName), "fake-binary");
        // ffprobe deliberadamente não criado

        var prev = Environment.GetEnvironmentVariable("FFMPEG_PATH");
        Environment.SetEnvironmentVariable("FFMPEG_PATH", tempDir);
        try
        {
            var sut = BuildSuccessSut(out var loggerMock);
            var context = Mock.Of<ILambdaContext>(ctx =>
                ctx.Logger == loggerMock.Object &&
                ctx.RemainingTime == TimeSpan.Zero);

            // Act — mesmo sem ffprobe, o handler processa (UseCase está mockado)
            var result = await sut.FunctionHandler(ValidInput(), context);

            result.Should().Contain("SUCCEEDED");
        }
        finally
        {
            Environment.SetEnvironmentVariable("FFMPEG_PATH", prev);
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true);
        }
    }
}
