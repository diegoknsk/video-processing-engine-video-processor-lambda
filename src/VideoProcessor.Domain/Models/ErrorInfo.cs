namespace VideoProcessor.Domain.Models;

public record ErrorInfo(
    string Type,
    string Message,
    bool Retryable
);
