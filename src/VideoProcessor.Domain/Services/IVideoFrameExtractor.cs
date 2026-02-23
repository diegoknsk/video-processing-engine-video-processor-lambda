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
    /// <returns>Resultado com total de frames, caminhos, duração do vídeo e tempo de processamento.</returns>
    Task<FrameExtractionResult> ExtractFramesAsync(
        string videoPath,
        int intervalSeconds,
        string outputFolder);
}
