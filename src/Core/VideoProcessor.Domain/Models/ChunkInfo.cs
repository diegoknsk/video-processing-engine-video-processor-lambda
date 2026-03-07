namespace VideoProcessor.Domain.Models;

public record ChunkInfo(
    string ChunkId,
    double StartSec,
    double EndSec,
    int IntervalSec = 1
);
