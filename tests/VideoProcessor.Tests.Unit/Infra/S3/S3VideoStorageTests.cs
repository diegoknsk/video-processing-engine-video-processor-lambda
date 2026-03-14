using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Moq;
using VideoProcessor.Infra.S3;
using Xunit;

namespace VideoProcessor.Tests.Unit.Infra.S3;

public class S3VideoStorageTests
{
    [Fact]
    public async Task DownloadToTempAsync_WhenS3Returns404_ThrowsFileNotFoundException()
    {
        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Not Found") { StatusCode = System.Net.HttpStatusCode.NotFound });

        var sut = new S3VideoStorage(s3Mock.Object);
        var tempPath = Path.Combine(Path.GetTempPath(), "S3VideoStorageTests", Guid.NewGuid().ToString("N"), "video.mp4");

        var act = () => sut.DownloadToTempAsync("bucket", "key", tempPath);

        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task DownloadToTempAsync_WhenS3Succeeds_WritesFileAndReturnsPath()
    {
        var testContent = new byte[] { 0x00, 0x01, 0x02 };
        var response = new GetObjectResponse
        {
            ResponseStream = new MemoryStream(testContent)
        };

        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var sut = new S3VideoStorage(s3Mock.Object);
        var tempDir = Path.Combine(Path.GetTempPath(), "S3VideoStorageTests", Guid.NewGuid().ToString("N"));
        var tempPath = Path.Combine(tempDir, "video.mp4");

        try
        {
            var result = await sut.DownloadToTempAsync("my-bucket", "path/to/video.mp4", tempPath);

            result.Should().Be(tempPath);
            File.Exists(result).Should().BeTrue();
            var bytes = await File.ReadAllBytesAsync(result);
            bytes.Should().BeEquivalentTo(testContent);
            s3Mock.Verify(x => x.GetObjectAsync(
                It.Is<GetObjectRequest>(r => r.BucketName == "my-bucket" && r.Key == "path/to/video.mp4"),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadFramesAsync_CallsPutObjectForEachFrameAndReturnsKeysInOrder()
    {
        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        var tempDir = Path.Combine(Path.GetTempPath(), "S3VideoStorageTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        var frame1 = Path.Combine(tempDir, "frame_0001_0s.jpg");
        var frame2 = Path.Combine(tempDir, "frame_0002_5s.jpg");
        var frame3 = Path.Combine(tempDir, "frame_0003_10s.jpg");
        await File.WriteAllTextAsync(frame1, "x");
        await File.WriteAllTextAsync(frame2, "x");
        await File.WriteAllTextAsync(frame3, "x");

        try
        {
            var sut = new S3VideoStorage(s3Mock.Object);
            var framePaths = new[] { frame1, frame2, frame3 };

            var result = await sut.UploadFramesAsync("bucket", "prefix/frames/", framePaths);

            result.Should().HaveCount(3);
            result[0].Should().Be("prefix/frames/frame_0001_0s.jpg");
            result[1].Should().Be("prefix/frames/frame_0002_5s.jpg");
            result[2].Should().Be("prefix/frames/frame_0003_10s.jpg");
            s3Mock.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadFramesAsync_NormalizesPrefixWithoutTrailingSlash()
    {
        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        var tempDir = Path.Combine(Path.GetTempPath(), "S3VideoStorageTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        var framePath = Path.Combine(tempDir, "frame_0001_0s.jpg");
        await File.WriteAllTextAsync(framePath, "x");

        try
        {
            var sut = new S3VideoStorage(s3Mock.Object);
            var result = await sut.UploadFramesAsync("bucket", "processed/video-abc/chunk-001/frames", [framePath]);

            result.Should().ContainSingle();
            result[0].Should().Be("processed/video-abc/chunk-001/frames/frame_0001_0s.jpg");
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadFramesAsync_WithEmptyPrefix_ReturnsJustFileName()
    {
        // Cobre o branch !string.IsNullOrEmpty(prefixNormalized) == false (prefix vazio)
        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        var tempDir = Path.Combine(Path.GetTempPath(), "S3VideoStorageTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        var framePath = Path.Combine(tempDir, "frame_0001_0s.jpg");
        await File.WriteAllTextAsync(framePath, "x");

        try
        {
            var sut = new S3VideoStorage(s3Mock.Object);
            var result = await sut.UploadFramesAsync("bucket", "", [framePath]);

            result.Should().ContainSingle();
            result[0].Should().Be("frame_0001_0s.jpg");
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task DownloadToTempAsync_WhenPathHasNoDirectory_SkipsCreateDirectoryAndPropagatesS3Error()
    {
        // Arrange — "video.mp4" sem componente de diretório → Path.GetDirectoryName retorna ""
        // → string.IsNullOrEmpty("") == true → Directory.CreateDirectory NÃO é chamado (branch false coberto)
        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Not Found") { StatusCode = System.Net.HttpStatusCode.NotFound });

        var sut = new S3VideoStorage(s3Mock.Object);

        // Act
        var act = () => sut.DownloadToTempAsync("bucket", "key", "video.mp4");

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task DownloadToTempAsync_WhenS3ReturnsForbidden_PropagatesOriginalAmazonS3Exception()
    {
        // Cobre o caminho onde AmazonS3Exception com status != 404 não é capturada pelo filtro when
        var s3Mock = new Mock<IAmazonS3>();
        s3Mock
            .Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Access Denied") { StatusCode = System.Net.HttpStatusCode.Forbidden });

        var sut = new S3VideoStorage(s3Mock.Object);
        var tempPath = Path.Combine(Path.GetTempPath(), "S3VideoStorageTests", Guid.NewGuid().ToString("N"), "video.mp4");

        var act = () => sut.DownloadToTempAsync("bucket", "key", tempPath);

        await act.Should().ThrowAsync<AmazonS3Exception>()
            .WithMessage("*Access Denied*");
    }
}
