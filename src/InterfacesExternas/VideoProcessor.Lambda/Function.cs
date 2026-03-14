using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using VideoProcessor.Application.Services;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Exceptions;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Domain.Services;
using VideoProcessor.Infra.S3;
using Xabe.FFmpeg;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace VideoProcessor.Lambda;

public class Function
{
    private readonly ProcessChunkUseCase _useCase;
    private static bool _ffmpegEnsured;

    [ExcludeFromCodeCoverage(Justification = "Wiring de DI e bootstrap da Lambda; testado via construtor interno em testes unitários.")]
    public Function()
    {
        TrySetFfmpegPathFromEnvOrKnownPaths();

        var services = new ServiceCollection();
        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client());
        services.AddSingleton<IS3VideoStorage, S3VideoStorage>();
        services.AddSingleton<IVideoFrameExtractor>(_ => new VideoFrameExtractor());
        services.AddSingleton<ProcessChunkUseCase>();
        var sp = services.BuildServiceProvider();
        _useCase = sp.GetRequiredService<ProcessChunkUseCase>();
    }

    /// <summary>
    /// Construtor para testes — injeta o UseCase diretamente e ignora o setup de FFmpeg.
    /// </summary>
    internal Function(ProcessChunkUseCase useCase)
    {
        _useCase = useCase;
        _ffmpegEnsured = true;
    }

    public async Task<string> FunctionHandler(ChunkProcessorInput input, ILambdaContext context)
    {
        TrySetFfmpegPathFromEnvOrKnownPaths();
        if (!IsFfmpegConfigured() && !_ffmpegEnsured)
        {
            await FFmpegSetup.EnsureFFmpegInstalledAsync();
            _ffmpegEnsured = true;
        }

        context.Logger.LogInformation("Iniciando processamento. VideoId={VideoId} ChunkId={ChunkId}",
            input.VideoId, input.Chunk.ChunkId);

        // RemainingTime pode ser Zero no Lambda Test Tool ou em contextos de teste → não cancelar por tempo
        var ct = context.RemainingTime > TimeSpan.FromSeconds(30)
            ? new CancellationTokenSource(context.RemainingTime).Token
            : CancellationToken.None;

        try
        {
            var output = await _useCase.ExecuteAsync(input, ct);

            context.Logger.LogInformation("Processamento concluído. Status={Status} Frames={Frames}",
                output.Status, output.FramesCount);

            return JsonSerializer.Serialize(output, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (VideoDurationSimulationException ex)
        {
            context.Logger.LogError(
                "[SIMULAÇÃO] VideoDurationSimulationException propagada até o Lambda handler. " +
                "VideoId={VideoId} ChunkId={ChunkId} DurationSeconds={DurationSeconds} Message={Message}",
                input.VideoId, input.Chunk.ChunkId, ex.DurationSeconds, ex.Message);
            throw;
        }
    }

    private static void TrySetFfmpegPathFromEnvOrKnownPaths()
    {
        var candidates = new List<string>();
        var envPath = Environment.GetEnvironmentVariable("FFMPEG_PATH");
        if (!string.IsNullOrWhiteSpace(envPath))
            candidates.Add(envPath.Trim());
        candidates.Add("/opt/bin");
        candidates.Add("/opt/ffmpeg");

        var ffmpegName = OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg";
        var ffprobeName = OperatingSystem.IsWindows() ? "ffprobe.exe" : "ffprobe";

        foreach (var dir in candidates)
        {
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                continue;
            if (File.Exists(Path.Combine(dir, ffmpegName)) && File.Exists(Path.Combine(dir, ffprobeName)))
            {
                FFmpeg.SetExecutablesPath(dir);
                return;
            }
        }
    }

    private static bool IsFfmpegConfigured()
    {
        var path = FFmpeg.ExecutablesPath;
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            return false;
        var ffmpegName = OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg";
        return File.Exists(Path.Combine(path, ffmpegName));
    }
}
