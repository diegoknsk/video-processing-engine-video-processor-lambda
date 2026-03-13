using System.Text;
using System.Text.Json;
using FluentAssertions;
using VideoProcessor.Lambda;
using Xunit;

namespace VideoProcessor.Tests.Unit.InterfacesExternas.Lambda;

public class JsonDocumentLambdaSerializerTests
{
    private readonly JsonDocumentLambdaSerializer _sut = new();

    [Fact]
    public void Deserialize_WhenTypeIsJsonDocument_ReturnsJsonDocument()
    {
        // Arrange
        var json = """{"key":"value","number":42}""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = _sut.Deserialize<JsonDocument>(stream);

        // Assert
        result.Should().NotBeNull();
        result.RootElement.GetProperty("key").GetString().Should().Be("value");
        result.RootElement.GetProperty("number").GetInt32().Should().Be(42);
    }

    [Fact]
    public void Deserialize_WhenTypeIsNotJsonDocument_ThrowsNotSupportedException()
    {
        // Arrange
        var json = """{"key":"value"}""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var act = () => _sut.Deserialize<string>(stream);

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*JsonDocumentLambdaSerializer only supports JsonDocument*");
    }

    [Fact]
    public void Serialize_WhenResponseIsJsonDocument_WritesJsonToOutputStream()
    {
        // Arrange
        var json = """{"status":"ok","count":3}""";
        using var inputDoc = JsonDocument.Parse(json);
        using var outputStream = new MemoryStream();

        // Act
        _sut.Serialize(inputDoc, outputStream);

        // Assert
        outputStream.Position = 0;
        var written = Encoding.UTF8.GetString(outputStream.ToArray());
        written.Should().Contain("status");
        written.Should().Contain("ok");
        written.Should().Contain("count");
        written.Should().Contain("3");
    }

    [Fact]
    public void Serialize_WhenResponseIsNotJsonDocument_ThrowsNotSupportedException()
    {
        // Arrange
        using var outputStream = new MemoryStream();

        // Act
        var act = () => _sut.Serialize("not a JsonDocument", outputStream);

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*JsonDocumentLambdaSerializer only supports JsonDocument*");
    }
}
