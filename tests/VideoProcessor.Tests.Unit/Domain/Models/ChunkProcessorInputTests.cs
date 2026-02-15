using System.Text.Json;
using FluentAssertions;
using VideoProcessor.Domain.Models;
using Xunit;

namespace VideoProcessor.Tests.Unit.Domain.Models;

public class ChunkProcessorInputTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void Deserialize_JsonCompleto_RetornaChunkProcessorInputComTodosOsCampos()
    {
        var json = """
            {
                "contractVersion": "1.0",
                "videoId": "vid-1",
                "chunk": { "chunkId": "chunk-0", "startSec": 0, "endSec": 10 },
                "source": { "bucket": "b1", "key": "v.mp4", "etag": "abc", "versionId": "v1" },
                "output": { "manifestBucket": "mb", "manifestPrefix": "man/", "framesBucket": "fb", "framesPrefix": "fr/" },
                "executionArn": "arn:aws:states:..."
            }
            """;

        var result = JsonSerializer.Deserialize<ChunkProcessorInput>(json, JsonOptions);

        result.Should().NotBeNull();
        result!.ContractVersion.Should().Be("1.0");
        result.VideoId.Should().Be("vid-1");
        result.Chunk.ChunkId.Should().Be("chunk-0");
        result.Chunk.StartSec.Should().Be(0);
        result.Chunk.EndSec.Should().Be(10);
        result.Source.Bucket.Should().Be("b1");
        result.Source.Key.Should().Be("v.mp4");
        result.Source.Etag.Should().Be("abc");
        result.Source.VersionId.Should().Be("v1");
        result.Output.ManifestBucket.Should().Be("mb");
        result.Output.FramesBucket.Should().Be("fb");
        result.ExecutionArn.Should().Be("arn:aws:states:...");
    }

    [Fact]
    public void Deserialize_JsonMinimo_SemCamposOpcionais_RetornaInputComOpcionaisNull()
    {
        var json = """
            {
                "contractVersion": "1.0",
                "videoId": "vid-1",
                "chunk": { "chunkId": "chunk-0", "startSec": 0, "endSec": 10 },
                "source": { "bucket": "b1", "key": "v.mp4" },
                "output": { "manifestBucket": "mb", "manifestPrefix": "man/" }
            }
            """;

        var result = JsonSerializer.Deserialize<ChunkProcessorInput>(json, JsonOptions);

        result.Should().NotBeNull();
        result!.Source.Etag.Should().BeNull();
        result.Source.VersionId.Should().BeNull();
        result.Output.FramesBucket.Should().BeNull();
        result.Output.FramesPrefix.Should().BeNull();
        result.ExecutionArn.Should().BeNull();
    }

    [Fact]
    public void Deserialize_JsonComCampoObrigatorioAusente_RetornaObjetoComObrigatoriosNull()
    {
        var json = """{ "contractVersion": "1.0", "videoId": "vid-1" }""";

        var result = JsonSerializer.Deserialize<ChunkProcessorInput>(json, JsonOptions);

        result.Should().NotBeNull();
        result!.Chunk.Should().BeNull();
        result.Source.Should().BeNull();
        result.Output.Should().BeNull();
    }

    [Fact]
    public void Deserialize_JsonInvalido_ChunkAusente_DeserializaComChunkNullOuFalha()
    {
        var json = """
            {
                "contractVersion": "1.0",
                "videoId": "vid-1",
                "source": { "bucket": "b1", "key": "v.mp4" },
                "output": { "manifestBucket": "mb", "manifestPrefix": "man/" }
            }
            """;

        var result = JsonSerializer.Deserialize<ChunkProcessorInput>(json, JsonOptions);

        result.Should().NotBeNull();
        result!.Chunk.Should().BeNull();
    }
}
