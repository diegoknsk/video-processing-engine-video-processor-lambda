namespace VideoProcessor.Domain.Models;

public record OutputConfig(
    string ManifestBucket,
    string ManifestPrefix,
    string? FramesBucket = null,
    string? FramesPrefix = null
);
