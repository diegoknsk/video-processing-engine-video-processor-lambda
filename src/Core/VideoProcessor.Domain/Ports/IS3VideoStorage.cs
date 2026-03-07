namespace VideoProcessor.Domain.Ports;

/// <summary>
/// Port para operações de armazenamento de vídeo em S3: download para arquivo local e upload de frames.
/// Agnóstico a AWS — sem dependência de AWSSDK.
/// </summary>
public interface IS3VideoStorage
{
    /// <summary>
    /// Baixa o objeto do bucket/key para o caminho local informado.
    /// </summary>
    /// <param name="bucket">Nome do bucket S3.</param>
    /// <param name="key">Chave do objeto no bucket.</param>
    /// <param name="localTempPath">Caminho completo do arquivo local onde o conteúdo será gravado (o diretório pai será criado se não existir).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Caminho local onde o arquivo foi gravado (igual a <paramref name="localTempPath"/>).</returns>
    /// <exception cref="FileNotFoundException">Quando o objeto não existe no S3 (ex.: 404).</exception>
    Task<string> DownloadToTempAsync(string bucket, string key, string localTempPath, CancellationToken ct = default);

    /// <summary>
    /// Faz upload de todos os frames locais para o bucket no prefixo informado.
    /// </summary>
    /// <param name="bucket">Nome do bucket S3 de destino.</param>
    /// <param name="prefix">Prefixo (prefixo lógico) onde os objetos serão criados; será normalizado com barra final se necessário.</param>
    /// <param name="localFramePaths">Caminhos locais dos arquivos de frame a enviar.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista das S3 keys dos objetos criados, na mesma ordem dos frames.</returns>
    Task<IReadOnlyList<string>> UploadFramesAsync(string bucket, string prefix, IEnumerable<string> localFramePaths, CancellationToken ct = default);
}
