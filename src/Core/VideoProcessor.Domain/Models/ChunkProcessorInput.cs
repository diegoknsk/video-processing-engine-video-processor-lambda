namespace VideoProcessor.Domain.Models;

public record ChunkProcessorInput(
    string ContractVersion,
    string VideoId,
    ChunkInfo Chunk,
    SourceInfo Source,
    OutputConfig Output,
    string? ExecutionArn = null
);
