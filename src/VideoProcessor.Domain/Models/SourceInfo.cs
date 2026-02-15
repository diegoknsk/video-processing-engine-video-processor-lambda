namespace VideoProcessor.Domain.Models;

public record SourceInfo(
    string Bucket,
    string Key,
    string? Etag = null,
    string? VersionId = null
);
