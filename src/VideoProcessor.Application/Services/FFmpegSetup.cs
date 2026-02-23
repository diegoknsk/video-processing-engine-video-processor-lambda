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
    /// caso contrário, baixa para %USERPROFILE%\.ffmpeg (Windows) e configura o caminho.
    /// </summary>
    public static async Task EnsureFFmpegInstalledAsync()
    {
        if (!string.IsNullOrWhiteSpace(FFmpeg.ExecutablesPath) && Directory.Exists(FFmpeg.ExecutablesPath))
            return;

        var ffmpegDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".ffmpeg");
        Directory.CreateDirectory(ffmpegDir);
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegDir);
        FFmpeg.SetExecutablesPath(ffmpegDir);
    }

    /// <summary>
    /// Retorna o caminho onde o FFmpeg está instalado/configurado.
    /// </summary>
    public static string GetFFmpegPath() => FFmpeg.ExecutablesPath ?? string.Empty;
}
