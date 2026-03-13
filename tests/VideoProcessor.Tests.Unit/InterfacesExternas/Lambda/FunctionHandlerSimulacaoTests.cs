using Amazon.Lambda.Core;
using FluentAssertions;
using Moq;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Exceptions;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Domain.Services;
using VideoProcessor.Lambda;
using Xunit;

namespace VideoProcessor.Tests.Unit.InterfacesExternas.Lambda;

public class FunctionHandlerSimulacaoTests
{
    private static ChunkProcessorInput ValidInput() =>
        new(
            ContractVersion: "1.0",
            VideoId: "video-simulacao",
            Chunk: new ChunkInfo("chunk-001", 0, 1303, 1),
            Source: new SourceInfo("bucket-src", "videos/simulacao.mp4"),
            Output: new OutputConfig("bucket-out", "frames/", FramesBucket: "bucket-out", FramesPrefix: "frames/"));

    private static (Function sut, Mock<ILambdaLogger> loggerMock) BuildSut(Exception exceptionToThrow)
    {
        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ThrowsAsync(exceptionToThrow);

        var useCase = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var sut = new Function(useCase);

        var loggerMock = new Mock<ILambdaLogger>(MockBehavior.Loose);
        return (sut, loggerMock);
    }

    private static ILambdaContext BuildContext(ILambdaLogger logger) =>
        Mock.Of<ILambdaContext>(ctx =>
            ctx.Logger == logger &&
            ctx.RemainingTime == TimeSpan.Zero);

    [Fact]
    public async Task FunctionHandler_WhenUseCaseThrowsVideoDurationSimulationException_RethrowsException()
    {
        // Arrange
        var simulationException = new VideoDurationSimulationException(1303.0);
        var (sut, loggerMock) = BuildSut(simulationException);
        var context = BuildContext(loggerMock.Object);

        // Act
        var act = () => sut.FunctionHandler(ValidInput(), context);

        // Assert — catch block executa e a exceção é relançada (não engolida)
        await act.Should().ThrowAsync<VideoDurationSimulationException>()
            .WithMessage("*SIMULAÇÃO*")
            .Where(ex => (int)ex.DurationSeconds == VideoDurationSimulationException.TriggerDurationSeconds);
    }

    [Fact]
    public async Task FunctionHandler_WhenUseCaseThrowsVideoDurationSimulationException_LogsBeforeRethrowing()
    {
        // Arrange
        var simulationException = new VideoDurationSimulationException(1303.0);
        var (sut, loggerMock) = BuildSut(simulationException);
        var context = BuildContext(loggerMock.Object);

        // Act
        await Assert.ThrowsAsync<VideoDurationSimulationException>(
            () => sut.FunctionHandler(ValidInput(), context));

        // Assert — LogError foi chamado com a mensagem de simulação
        loggerMock.Verify(
            x => x.LogError(It.Is<string>(msg => msg.Contains("SIMULAÇÃO")), It.IsAny<object[]>()),
            Times.Once);
    }
}
