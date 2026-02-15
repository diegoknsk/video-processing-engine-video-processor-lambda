namespace VideoProcessor.Domain.Models;

public record ChunkProcessorOutput(
    string ChunkId,
    ProcessingStatus Status,
    int FramesCount,
    ManifestInfo? Manifest = null,
    ErrorInfo? Error = null
);
