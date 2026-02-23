using System.Diagnostics;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Services;
using Xabe.FFmpeg;

namespace VideoProcessor.Application.Services;

/// <summary>
/// Implementação de <see cref="IVideoFrameExtractor"/> usando Xabe.FFmpeg para extrair frames em intervalos fixos.
/// </summary>
public class VideoFrameExtractor : IVideoFrameExtractor
{
    private readonly IProgress<(int Current, int Total)>? _progress;

    public VideoFrameExtractor(IProgress<(int Current, int Total)>? progress = null)
    {
        _progress = progress;
    }

    /// <inheritdoc />
    public async Task<FrameExtractionResult> ExtractFramesAsync(
        string videoPath,
        int intervalSeconds,
        string outputFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(videoPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFolder);
        if (intervalSeconds < 1)
            throw new ArgumentOutOfRangeException(nameof(intervalSeconds), "Intervalo deve ser >= 1 segundo.");

        if (!File.Exists(videoPath))
            throw new FileNotFoundException("Arquivo de vídeo não encontrado.", videoPath);

        Directory.CreateDirectory(outputFolder);

        var stopwatch = Stopwatch.StartNew();
        IMediaInfo mediaInfo;
        try
        {
            mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Não foi possível obter informações do vídeo (formato pode não ser suportado): {ex.Message}", ex);
        }

        var duration = mediaInfo.Duration;
        var totalFrames = (int)(duration.TotalSeconds / intervalSeconds);
        if (totalFrames == 0)
            totalFrames = 1;

        var framePaths = new List<string>(totalFrames);

        for (var i = 0; i < totalFrames; i++)
        {
            var currentTime = TimeSpan.FromSeconds(i * intervalSeconds);
            var secondsLabel = (int)currentTime.TotalSeconds;
            var fileName = $"frame_{(i + 1):D4}_{secondsLabel}s.jpg";
            var outputPath = Path.Combine(outputFolder, fileName);

            var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoPath, outputPath, currentTime);
            await conversion.Start();

            framePaths.Add(outputPath);
            _progress?.Report((i + 1, totalFrames));
        }

        stopwatch.Stop();
        return new FrameExtractionResult(
            TotalFrames: framePaths.Count,
            FramePaths: framePaths,
            VideoDuration: duration,
            ProcessingDuration: stopwatch.Elapsed);
    }
}
