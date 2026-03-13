using FluentAssertions;
using VideoProcessor.Domain.Models;
using Xunit;

namespace VideoProcessor.Tests.Unit.Domain.Models;

public class FrameExtractionResultTests
{
    [Fact]
    public void FrameExtractionResult_WhenCreated_ShouldExposeAllPropertiesCorrectly()
    {
        // Arrange
        var framePaths = new List<string> { "/tmp/frame_0001_0s.jpg", "/tmp/frame_0002_5s.jpg" };
        var videoDuration = TimeSpan.FromSeconds(30);
        var processingDuration = TimeSpan.FromMilliseconds(850);

        // Act
        var result = new FrameExtractionResult(
            TotalFrames: 2,
            FramePaths: framePaths,
            VideoDuration: videoDuration,
            ProcessingDuration: processingDuration);

        // Assert
        result.TotalFrames.Should().Be(2);
        result.FramePaths.Should().BeEquivalentTo(framePaths);
        result.VideoDuration.Should().Be(videoDuration);
        result.ProcessingDuration.Should().Be(processingDuration);
    }

    [Fact]
    public void FrameExtractionResult_TwoInstancesWithSameValues_ShouldBeEqual()
    {
        // Arrange
        var framePaths = new List<string> { "/tmp/frame_0001_0s.jpg" };
        var duration = TimeSpan.FromSeconds(10);
        var processing = TimeSpan.FromMilliseconds(100);

        var a = new FrameExtractionResult(1, framePaths, duration, processing);
        var b = new FrameExtractionResult(1, framePaths, duration, processing);

        // Assert — record compara por valor
        a.Should().Be(b);
    }

    [Fact]
    public void FrameExtractionResult_WithDifferentTotalFrames_ShouldNotBeEqual()
    {
        // Arrange
        var framePaths = new List<string>();
        var duration = TimeSpan.FromSeconds(10);
        var processing = TimeSpan.FromMilliseconds(100);

        var a = new FrameExtractionResult(1, framePaths, duration, processing);
        var b = new FrameExtractionResult(2, framePaths, duration, processing);

        // Assert
        a.Should().NotBe(b);
    }
}
