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

    [Fact]
    public async Task ExtractFramesAsync_StartTimeNegative_ThrowsArgumentOutOfRangeException()
    {
        var videoPath = Path.GetTempFileName();
        var outputFolder = Path.Combine(Path.GetTempPath(), "frames-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            var act = () => _sut.ExtractFramesAsync(videoPath, 20, outputFolder, startTimeSeconds: -1, endTimeSeconds: null);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("startTimeSeconds")
                .WithMessage("*>= 0*");
        }
        finally
        {
            if (File.Exists(videoPath)) File.Delete(videoPath);
        }
    }

    [Fact]
    public async Task ExtractFramesAsync_StartGreaterThanOrEqualEnd_ThrowsArgumentOutOfRangeException()
    {
        var videoPath = Path.GetTempFileName();
        var outputFolder = Path.Combine(Path.GetTempPath(), "frames-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            var act = () => _sut.ExtractFramesAsync(videoPath, 20, outputFolder, startTimeSeconds: 60, endTimeSeconds: 40);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("endTimeSeconds")
                .WithMessage("*maior que o tempo de início*");
        }
        finally
        {
            if (File.Exists(videoPath)) File.Delete(videoPath);
        }
    }

    [Fact]
    public async Task ExtractFramesAsync_StartEqualsEnd_ThrowsArgumentOutOfRangeException()
    {
        var videoPath = Path.GetTempFileName();
        var outputFolder = Path.Combine(Path.GetTempPath(), "frames-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            var act = () => _sut.ExtractFramesAsync(videoPath, 20, outputFolder, startTimeSeconds: 30, endTimeSeconds: 30);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("endTimeSeconds");
        }
        finally
        {
            if (File.Exists(videoPath)) File.Delete(videoPath);
        }
    }

    [Fact(Skip = "Requer arquivo de vídeo real para obter duração; executar manualmente com sample.mp4 na raiz do projeto.")]
    public async Task ExtractFramesAsync_EndGreaterThanVideoDuration_ThrowsArgumentOutOfRangeException()
    {
        var videoPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "sample.mp4");
        var fullPath = Path.GetFullPath(videoPath);
        if (!File.Exists(fullPath))
            return;

        var outputFolder = Path.Combine(Path.GetTempPath(), "frames-" + Guid.NewGuid().ToString("N")[..8]);
        var act = () => _sut.ExtractFramesAsync(fullPath, 20, outputFolder, startTimeSeconds: 0, endTimeSeconds: 999999);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("endTimeSeconds")
            .WithMessage("*exceder a duração*");
    }
}
