using FluentAssertions;
using Moq;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Exceptions;
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
    public async Task ExecuteAsync_ExtractorThrowsVideoDurationSimulationException_PropagatesWithoutCatching()
    {
        // Arrange
        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);

        var simulationException = new VideoDurationSimulationException(1303.0);
        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ThrowsAsync(simulationException);

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput();

        // Act
        var act = () => sut.ExecuteAsync(input);

        // Assert — não capturada como FAILED, propaga até o caller (Lambda handler)
        await act.Should().ThrowAsync<VideoDurationSimulationException>()
            .WithMessage("*SIMULAÇÃO*")
            .Where(ex => (int)ex.DurationSeconds == VideoDurationSimulationException.TriggerDurationSeconds);
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

    [Fact]
    public async Task ExecuteAsync_InvalidInputEmptyKey_ReturnsFailed()
    {
        // Cobre a condição string.IsNullOrWhiteSpace(source.Key)
        var extractorMock = new Mock<IVideoFrameExtractor>();
        var storageMock = new Mock<IS3VideoStorage>();
        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with { Source = new SourceInfo("bucket", "") };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.Error!.Type.Should().Be("INVALID_INPUT");
        storageMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenFramesBucketAndManifestBucketAreNull_ReturnsFailed()
    {
        // Cobre a condição string.IsNullOrWhiteSpace(framesBucket)
        var extractorMock = new Mock<IVideoFrameExtractor>();
        var storageMock = new Mock<IS3VideoStorage>();
        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with
        {
            Output = new OutputConfig(ManifestBucket: null!, ManifestPrefix: "prefix/", FramesBucket: null, FramesPrefix: "frames/")
            // ManifestBucket=null força framesBucket a ser null (ambos null)
        };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.Error!.Type.Should().Be("INVALID_INPUT");
        storageMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenFramesPrefixAndManifestPrefixAreEmpty_ReturnsFailed()
    {
        // Cobre a condição string.IsNullOrWhiteSpace(framesPrefix)
        var extractorMock = new Mock<IVideoFrameExtractor>();
        var storageMock = new Mock<IS3VideoStorage>();
        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with
        {
            Output = new OutputConfig("bucket-out", ManifestPrefix: null!, FramesBucket: "bucket-out", FramesPrefix: null)
            // ManifestPrefix=null! e FramesPrefix=null → framesPrefix = "" → validação falha
        };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.FAILED);
        result.Error!.Type.Should().Be("INVALID_INPUT");
        storageMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenManifestPrefixHasNoTrailingSlash_NormalizesKeyWithSlash()
    {
        // Cobre o branch de normalização do manifestPrefix (linha: manifestPrefix += "/")
        var framePaths = new List<string> { "/tmp/f1.jpg" };
        var extractResult = new FrameExtractionResult(1, framePaths, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);
        storageMock
            .Setup(x => x.UploadFramesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["frames/f1.jpg"]);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(extractResult);

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        // ManifestPrefix sem trailing slash — deve ser normalizado para "manifests/manifest.json"
        var input = ValidInput() with
        {
            Output = new OutputConfig("bucket-out", ManifestPrefix: "manifests", FramesBucket: "bucket-out", FramesPrefix: "frames/")
        };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.SUCCEEDED);
        result.Manifest.Should().NotBeNull();
        result.Manifest!.Key.Should().Be("manifests/manifest.json");
    }

    [Fact]
    public async Task ExecuteAsync_WhenManifestPrefixIsNull_UsesEmptyPrefix()
    {
        // Cobre o branch where manifestPrefix is empty (sem normalização)
        var framePaths = new List<string> { "/tmp/f1.jpg" };
        var extractResult = new FrameExtractionResult(1, framePaths, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);
        storageMock
            .Setup(x => x.UploadFramesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["frames/f1.jpg"]);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(extractResult);

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with
        {
            Output = new OutputConfig("bucket-out", ManifestPrefix: null!, FramesBucket: "bucket-out", FramesPrefix: "frames/")
            // ManifestPrefix=null → manifestKey = "manifest.json" (sem prefix)
        };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.SUCCEEDED);
        result.Manifest.Should().NotBeNull();
        result.Manifest!.Key.Should().Be("manifest.json");
    }

    [Fact]
    public async Task ExecuteAsync_WhenFramesPrefixNullButManifestPrefixSet_UsesManifestPrefixAsFramesPrefix()
    {
        // Cobre o branch: output.FramesPrefix ?? output.ManifestPrefix onde FramesPrefix=null e ManifestPrefix tem valor
        var framePaths = new List<string> { "/tmp/f1.jpg" };
        var extractResult = new FrameExtractionResult(1, framePaths, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);
        storageMock
            .Setup(x => x.UploadFramesAsync("bucket-out", "manifests/", It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["manifests/f1.jpg"]);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(extractResult);

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with
        {
            Output = new OutputConfig("bucket-out", ManifestPrefix: "manifests/", FramesBucket: "bucket-out", FramesPrefix: null)
        };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.SUCCEEDED);
        storageMock.Verify(
            x => x.UploadFramesAsync("bucket-out", "manifests/", It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFramesBucketNullButManifestBucketSet_UsesManifestBucketForFrames()
    {
        // Cobre o branch: output.FramesBucket ?? output.ManifestBucket onde FramesBucket=null e ManifestBucket tem valor
        var framePaths = new List<string> { "/tmp/f1.jpg" };
        var extractResult = new FrameExtractionResult(1, framePaths, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

        var storageMock = new Mock<IS3VideoStorage>();
        storageMock
            .Setup(x => x.DownloadToTempAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string path, CancellationToken _) => path);
        storageMock
            .Setup(x => x.UploadFramesAsync("manifest-bucket", "frames/", It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["frames/f1.jpg"]);

        var extractorMock = new Mock<IVideoFrameExtractor>();
        extractorMock
            .Setup(x => x.ExtractFramesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(extractResult);

        var sut = new ProcessChunkUseCase(extractorMock.Object, storageMock.Object);
        var input = ValidInput() with
        {
            Output = new OutputConfig("manifest-bucket", ManifestPrefix: "manifests/", FramesBucket: null, FramesPrefix: "frames/")
        };

        var result = await sut.ExecuteAsync(input);

        result.Status.Should().Be(ProcessingStatus.SUCCEEDED);
        storageMock.Verify(
            x => x.UploadFramesAsync("manifest-bucket", "frames/", It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
