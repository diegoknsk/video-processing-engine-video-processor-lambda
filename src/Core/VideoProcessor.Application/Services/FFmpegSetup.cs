using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace VideoProcessor.Application.Services;

/// <summary>
/// Helper para garantir que o FFmpeg está instalado e configurado (download na primeira execução se necessário).
/// </summary>
public static class FFmpegSetup
{
    /// <summary>
    /// Garante que o FFmpeg está disponível: se já houver binários configurados, não faz nada;
    /// caso contrário, baixa para o diretório apropriado (em Lambda usa /tmp/.ffmpeg; localmente %USERPROFILE%\.ffmpeg) e configura o caminho.
    /// </summary>
    public static async Task EnsureFFmpegInstalledAsync()
    {
        if (!string.IsNullOrWhiteSpace(FFmpeg.ExecutablesPath) && Directory.Exists(FFmpeg.ExecutablesPath))
            return;

        var basePath = IsLambdaEnvironment()
            ? "/tmp"
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var ffmpegDir = Path.Combine(basePath, ".ffmpeg");
        Directory.CreateDirectory(ffmpegDir);
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegDir);
        FFmpeg.SetExecutablesPath(ffmpegDir);
    }

    /// <summary>
    /// Indica se a aplicação está rodando em ambiente AWS Lambda (sistema de arquivos somente leitura em /var/task).
    /// </summary>
    internal static bool IsLambdaEnvironment() =>
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"))
        || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT"));

    /// <summary>
    /// Retorna o caminho onde o FFmpeg está instalado/configurado.
    /// </summary>
    public static string GetFFmpegPath() => FFmpeg.ExecutablesPath ?? string.Empty;
}
