using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using VideoProcessor.Domain.Models;
using Xunit;

namespace VideoProcessor.Tests.Unit.Domain.Models;

public class ChunkProcessorOutputTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void Serialize_StatusSucceeded_ContemManifest_NaoContemError()
    {
        var output = new ChunkProcessorOutput(
            ChunkId: "chunk-0",
            Status: ProcessingStatus.SUCCEEDED,
            FramesCount: 5,
            Manifest: new ManifestInfo("bucket", "prefix/manifest.json")
        );

        var json = JsonSerializer.Serialize(output, JsonOptions);

        output.Manifest.Should().NotBeNull();
        output.Error.Should().BeNull();
        json.Should().Contain("manifest");
        json.Should().Contain("SUCCEEDED");
        json.Should().NotContain("error");
    }

    [Fact]
    public void Serialize_StatusFailed_ContemError_NaoContemManifest()
    {
        var output = new ChunkProcessorOutput(
            ChunkId: "chunk-0",
            Status: ProcessingStatus.FAILED,
            FramesCount: 0,
            Error: new ErrorInfo("ValidationError", "Invalid version", Retryable: false)
        );

        var json = JsonSerializer.Serialize(output, JsonOptions);

        output.Error.Should().NotBeNull();
        output.Manifest.Should().BeNull();
        json.Should().Contain("error");
        json.Should().Contain("FAILED");
        json.Should().NotContain("manifest");
    }

    [Fact]
    public void Serialize_ProcessingStatus_SerializaComoStringNaoNumero()
    {
        var output = new ChunkProcessorOutput("chunk-0", ProcessingStatus.SUCCEEDED, 0);

        var json = JsonSerializer.Serialize(output, JsonOptions);

        json.Should().Contain("\"status\":\"SUCCEEDED\"");
        json.Should().NotContain("\"status\":0");
    }

    [Fact]
    public void Serialize_CamposNull_SaoOmitidosDoJson()
    {
        var output = new ChunkProcessorOutput("chunk-0", ProcessingStatus.FAILED, 0, Error: new ErrorInfo("T", "M", false));

        var json = JsonSerializer.Serialize(output, JsonOptions);

        json.Should().NotContain("manifest");
        json.Should().Contain("error");
    }
}
