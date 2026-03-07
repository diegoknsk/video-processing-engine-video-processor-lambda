namespace VideoProcessor.Domain.Models;

/// <summary>
/// Resultado da extração de frames de um vídeo.
/// </summary>
/// <param name="TotalFrames">Quantidade total de frames extraídos.</param>
/// <param name="FramePaths">Caminhos completos dos arquivos de frame gerados.</param>
/// <param name="VideoDuration">Duração total do vídeo.</param>
/// <param name="ProcessingDuration">Tempo gasto no processamento.</param>
public record FrameExtractionResult(
    int TotalFrames,
    List<string> FramePaths,
    TimeSpan VideoDuration,
    TimeSpan ProcessingDuration
);
