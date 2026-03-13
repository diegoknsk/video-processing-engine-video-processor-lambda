using FluentAssertions;
using VideoProcessor.Domain.Exceptions;
using Xunit;

namespace VideoProcessor.Tests.Unit.Domain.Exceptions;

public class VideoDurationSimulationExceptionTests
{
    [Fact]
    public void Constructor_WithDuration1303_SetsDurationSecondsProperty()
    {
        var sut = new VideoDurationSimulationException(1303.0);

        sut.DurationSeconds.Should().Be(1303.0);
    }

    [Fact]
    public void Constructor_WithDuration1303_MessageContainsSimulacaoAndDuration()
    {
        var sut = new VideoDurationSimulationException(1303.0);

        sut.Message.Should().Contain("SIMULAÇÃO");
        sut.Message.Should().Contain("1303");
    }

    [Fact]
    public void Constructor_InheritsFromException()
    {
        var sut = new VideoDurationSimulationException(1303.0);

        sut.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void TriggerDurationSeconds_Constant_IsEqualTo1303()
    {
        VideoDurationSimulationException.TriggerDurationSeconds.Should().Be(1303);
    }

    [Theory]
    [InlineData(1303.0)]
    [InlineData(1303.5)]
    [InlineData(1303.99)]
    public void Constructor_WithAnyDurationValue_PreservesDurationSecondsExact(double duration)
    {
        var sut = new VideoDurationSimulationException(duration);

        sut.DurationSeconds.Should().Be(duration);
    }
}
