using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using VideoProcessor.Application.Services;
using VideoProcessor.Domain.Exceptions;
using VideoProcessor.Domain.Models;

[assembly: LambdaSerializer(typeof(VideoProcessor.Lambda.JsonDocumentLambdaSerializer))]

namespace VideoProcessor.Lambda;

public class Function
{
    private readonly IContractVersionValidator _versionValidator;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public Function()
    {
        var serviceProvider = ConfigureServices();
        _versionValidator = serviceProvider.GetRequiredService<IContractVersionValidator>();
    }

    public Task<JsonDocument> FunctionHandler(JsonDocument inputDoc, ILambdaContext context)
    {
        ChunkProcessorInput? input = null;
        try
        {
            var inputJson = inputDoc.RootElement.GetRawText();
            input = JsonSerializer.Deserialize<ChunkProcessorInput>(inputJson, _jsonOptions);
            if (input is null)
                throw new InvalidOperationException("Failed to deserialize input");

            _versionValidator.Validate(input.ContractVersion);

            context.Logger.LogInformation("Processing videoId={VideoId}, chunkId={ChunkId}", input.VideoId, input.Chunk.ChunkId);

            var output = new ChunkProcessorOutput(
                ChunkId: input.Chunk.ChunkId,
                Status: ProcessingStatus.SUCCEEDED,
                FramesCount: 0,
                Manifest: new ManifestInfo(
                    input.Output.ManifestBucket,
                    $"{input.Output.ManifestPrefix.TrimEnd('/')}/{input.Chunk.ChunkId}/manifest.json"
                )
            );
            var outputJson = JsonSerializer.Serialize(output, _jsonOptions);
            return Task.FromResult(JsonDocument.Parse(outputJson));
        }
        catch (UnsupportedContractVersionException ex)
        {
            context.Logger.LogError("Unsupported contract version: {Message}", ex.Message);
            var errorOutput = new ChunkProcessorOutput(
                ChunkId: input?.Chunk?.ChunkId ?? "unknown",
                Status: ProcessingStatus.FAILED,
                FramesCount: 0,
                Error: new ErrorInfo("ValidationError", ex.Message, Retryable: false)
            );
            var outputJson = JsonSerializer.Serialize(errorOutput, _jsonOptions);
            return Task.FromResult(JsonDocument.Parse(outputJson));
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IContractVersionValidator, ContractVersionValidator>();
        return services.BuildServiceProvider();
    }
}
