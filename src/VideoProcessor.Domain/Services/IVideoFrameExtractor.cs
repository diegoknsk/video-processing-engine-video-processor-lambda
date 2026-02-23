using VideoProcessor.Domain.Models;

namespace VideoProcessor.Domain.Services;

/// <summary>
/// Port para extração de frames de vídeos em intervalos parametrizáveis.
/// </summary>
public interface IVideoFrameExtractor
{
    /// <summary>
    /// Extrai frames do vídeo no caminho indicado, em intervalos de tempo fixos, salvando na pasta de saída.
    /// </summary>
    /// <param name="videoPath">Caminho completo do arquivo de vídeo.</param>
    /// <param name="intervalSeconds">Intervalo em segundos entre cada frame (ex: 20 = um frame a cada 20s).</param>
    /// <param name="outputFolder">Pasta onde os frames serão salvos (será criada se não existir).</param>
    /// <param name="startTimeSeconds">Opcional: tempo de início do trecho em segundos. Quando omitido (null), processa desde o início do vídeo.</param>
    /// <param name="endTimeSeconds">Opcional: tempo de fim do trecho em segundos. Quando omitido (null), processa até o fim do vídeo. Quando informado junto com start, apenas o trecho [start, end] é processado.</param>
    /// <returns>Resultado com total de frames, caminhos, duração do vídeo e tempo de processamento.</returns>
    /// <remarks>Quando ambos startTimeSeconds e endTimeSeconds são null/omitidos, o comportamento é processar o vídeo inteiro. Quando informados, processa apenas o trecho [start, end] em segundos.</remarks>
    Task<FrameExtractionResult> ExtractFramesAsync(
        string videoPath,
        int intervalSeconds,
        string outputFolder,
        int? startTimeSeconds = null,
        int? endTimeSeconds = null);
}
