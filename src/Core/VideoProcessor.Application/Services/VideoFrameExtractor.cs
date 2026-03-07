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
        string outputFolder,
        int? startTimeSeconds = null,
        int? endTimeSeconds = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(videoPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFolder);
        if (intervalSeconds < 1)
            throw new ArgumentOutOfRangeException(nameof(intervalSeconds), "Intervalo deve ser >= 1 segundo.");

        if (!File.Exists(videoPath))
            throw new FileNotFoundException("Arquivo de vídeo não encontrado.", videoPath);

        if (startTimeSeconds is { } startVal && startVal < 0)
            throw new ArgumentOutOfRangeException(nameof(startTimeSeconds), startTimeSeconds, "Tempo de início deve ser >= 0.");
        if (startTimeSeconds.HasValue && endTimeSeconds.HasValue && startTimeSeconds.Value >= endTimeSeconds.Value)
            throw new ArgumentOutOfRangeException(nameof(endTimeSeconds), endTimeSeconds, "Tempo de fim deve ser maior que o tempo de início.");

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
        var durationSeconds = (int)Math.Floor(duration.TotalSeconds);

        var startE = startTimeSeconds ?? 0;
        var endE = endTimeSeconds ?? durationSeconds;

        if (endE > durationSeconds)
            throw new ArgumentOutOfRangeException(nameof(endTimeSeconds), endTimeSeconds, "Tempo de fim não pode exceder a duração do vídeo.");

        var totalFrames = (endE - startE) / intervalSeconds + 1;
        if (totalFrames < 1)
            totalFrames = 1;

        var framePaths = new List<string>(totalFrames);

        for (var i = 0; i < totalFrames; i++)
        {
            var currentSeconds = startE + (i * intervalSeconds);
            var currentTime = TimeSpan.FromSeconds(currentSeconds);
            var fileName = $"frame_{(i + 1):D4}_{currentSeconds}s.jpg";
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
