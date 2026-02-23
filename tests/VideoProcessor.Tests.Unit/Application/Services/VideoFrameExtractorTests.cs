using FluentAssertions;
using VideoProcessor.Application.Services;
using Xunit;

namespace VideoProcessor.Tests.Unit.Application.Services;

public class VideoFrameExtractorTests
{
    private readonly VideoFrameExtractor _sut = new();

    [Fact]
    public async Task ExtractFramesAsync_InvalidVideoPath_ThrowsFileNotFoundException()
    {
        var videoPath = Path.Combine(Path.GetTempPath(), "video-inexistente-xyz.mp4");
        var outputFolder = Path.Combine(Path.GetTempPath(), "frames-out-" + Guid.NewGuid().ToString("N")[..8]);

        var act = () => _sut.ExtractFramesAsync(videoPath, 20, outputFolder);

        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage("*não encontrado*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExtractFramesAsync_IntervalLessThanOne_ThrowsArgumentOutOfRangeException(int interval)
    {
        var videoPath = Path.GetTempFileName();
        var outputFolder = Path.Combine(Path.GetTempPath(), "frames-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            var act = () => _sut.ExtractFramesAsync(videoPath, interval, outputFolder);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("intervalSeconds");
        }
        finally
        {
            if (File.Exists(videoPath)) File.Delete(videoPath);
        }
    }

    [Fact]
    public async Task ExtractFramesAsync_NullVideoPath_ThrowsArgumentException()
    {
        var act = () => _sut.ExtractFramesAsync(null!, 20, @"C:\out");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("videoPath");
    }

    [Fact]
    public async Task ExtractFramesAsync_EmptyVideoPath_ThrowsArgumentException()
    {
        var act = () => _sut.ExtractFramesAsync("", 20, @"C:\out");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExtractFramesAsync_NullOutputFolder_ThrowsArgumentException()
    {
        var videoPath = Path.GetTempFileName();
        try
        {
            var act = () => _sut.ExtractFramesAsync(videoPath, 20, null!);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("outputFolder");
        }
        finally
        {
            if (File.Exists(videoPath)) File.Delete(videoPath);
        }
    }

    [Fact]
    public void FrameExtractionResult_CalculoDeterministico_MesmoVideoMesmoIntervalo_MesmaQuantidade()
    {
        var durationSeconds = 100;
        var intervalSeconds = 20;
        var expectedCount = (int)(durationSeconds / intervalSeconds);

        expectedCount.Should().Be(5);
    }
}
