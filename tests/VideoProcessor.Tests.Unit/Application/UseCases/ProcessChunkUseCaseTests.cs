using FluentAssertions;
using Moq;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Domain.Services;
using Xunit;

namespace VideoProcessor.Tests.Unit.Application.UseCases;

public class ProcessChunkUseCaseTests
{
    private static ChunkProcessorInput ValidInput() =>
        new(
            ContractVersion: "1.0",
            VideoId: "video-abc",
            Chunk: new ChunkInfo("chunk-001", 0, 60, 5),
            Source: new SourceInfo("bucket-src", "key/video.mp4"),
            Output: new OutputConfig("bucket-out", "prefix/", FramesBucket: "bucket-out", FramesPrefix: "prefix/frames/"));

    [Fact]
    public async Task ExecuteAsync_ValidInput_ReturnsSucceededAndFramesCount()
    {
        var framePaths = new List<string> { "/tmp/f1.jpg", "/tmp/f2.jpg", "/tmp/f3.jpg" };
        var extractResult = new FrameExtractionResult(3, framePaths, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(2));
        var uploadedKeys = new List<string> { "prefix/frames/f1.jpg", "prefix/frames/f2.jpg", "prefix/frames/f3.jpg" };

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), 5, It.IsAny<string>(), 0, 60))
            .ReturnsAsync(extractResult);

        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync("bucket-src", "key/video.mp4", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string bucket, string key, string path, CancellationToken _) => path);
        storageMock
            .Setup(x => x.UploadFramesAsync("bucket-out", "prefix/frames/", It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, IEnumerable<string> paths, CancellationToken _) => paths.Select(p => "prefix/frames/" + Path.GetFileName(p)).ToList());

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput();

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.SUCCEEDED);
        result.FramesCount.Should().Be(3);
        result.ChunkId.Should().Be("chunk-001");
        result.Error.Should().BeNull();
        result.Manifest.Should().NotBeNull();
        result.Manifest!.Bucket.Should().Be("bucket-out");
        result.Manifest.Key.Should().Contain("manifest.json");
    }

    [Fact]
    public async Task ExecuteAsync_NullInput_ReturnsFailedWithError()
    {
        var extractorMock = new Mock<IVideoFrameExtractor>();
        var storageMock = new Mock<IS3VideoStorage>();
        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);

        var result = await sut.ExecuteAsync(null!);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.FramesCount.Should().Be(0);
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Contain("nulo");
        extractorMock.VerifyNoOtherCalls();
        storageMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_DownloadThrowsFileNotFoundException_ReturnsFailedWithVideoNotFound()
    {
        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FileNotFoundException("Vídeo não encontrado no S3: s3://b/k"));

        var extractorMock = new Mock<IVideoFrameExtractor>();
        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput();

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.FramesCount.Should().Be(0);
        result.Error.Should().NotBeNull();
        result.Error!.Type.Should().Be("VIDEO_NOT_FOUND");
        result.Error.Message.Should().Contain("não encontrado");
        extractorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_ExtractorThrowsInvalidOperationException_ReturnsFailedWithExtractionError()
    {
        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ThrowsAsync(new InvalidOperationException("FFmpeg error"));

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput();

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.FramesCount.Should().Be(0);
        result.Error.Should().NotBeNull();
        result.Error!.Type.Should().Be("EXTRACTION_FAILED");
        result.Error.Message.Should().Contain("FFmpeg error");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidInputMissingSource_ReturnsFailed()
    {
        var extractorMock = new Mock<IVideoFrameExtractor>();
        var storageMock = new Mock<IS3VideoStorage>();
        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with { Source = new SourceInfo("", "key") };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.FramesCount.Should().Be(0);
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Contain("obrigatórios");
        storageMock.VerifyNoOtherCalls();
    }
}
