using System.Diagnostics.CodeAnalysis;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Domain.Services;

namespace VideoProcessor.Application.UseCases;

/// <summary>
/// Orquestra o processamento de um chunk de vídeo: download do S3, extração de frames e upload dos frames para S3.
/// Reutilizado pela Lambda e pelo CLI modo AWS.
/// </summary>
public class ProcessChunkUseCase(
    IVideoFrameExtractor frameExtractor,
    IS3VideoStorage s3Storage)
{
    /// <summary>
    /// Executa o processamento do chunk: download → extração → upload → cleanup.
    /// </summary>
    /// <param name="input">Input com origem S3, chunk e configuração de saída.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Resultado com status, quantidade de frames e manifest ou erro.</returns>
    public async Task<ChunkProcessorOutput> ExecuteAsync(ChunkProcessorInput input, CancellationToken ct = default)
    {
        if (input is null)
        {
            return new ChunkProcessorOutput(
                ChunkId: "",
                Status: ProcessingStatus.FAILED,
                FramesCount: 0,
                Error: new ErrorInfo("INVALID_INPUT", "Input não pode ser nulo.", Retryable: false));
        }

        var source = input.Source;
        var output = input.Output;
        var framesBucket = output.FramesBucket ?? output.ManifestBucket;
        var framesPrefix = output.FramesPrefix ?? output.ManifestPrefix ?? "";

        if (string.IsNullOrWhiteSpace(source?.Bucket) || string.IsNullOrWhiteSpace(source.Key) ||
            string.IsNullOrWhiteSpace(framesBucket) || string.IsNullOrWhiteSpace(framesPrefix))
        {
            return new ChunkProcessorOutput(
                input.Chunk.ChunkId,
                ProcessingStatus.FAILED,
                0,
                Error: new ErrorInfo("INVALID_INPUT", "Source.Bucket, Source.Key, Output.FramesBucket e Output.FramesPrefix são obrigatórios.", Retryable: false));
        }

        var videoId = input.VideoId;
        var chunkId = input.Chunk.ChunkId;
        var baseTempDir = Path.Combine(Path.GetTempPath(), videoId, chunkId);
        var videoTempDir = Path.Combine(baseTempDir, "video");
        var framesTempDir = Path.Combine(baseTempDir, "frames");
        var videoTempPath = Path.Combine(videoTempDir, Path.GetFileName(source.Key));

        try
        {
            Directory.CreateDirectory(videoTempDir);
            Directory.CreateDirectory(framesTempDir);

            await s3Storage.DownloadToTempAsync(source.Bucket, source.Key, videoTempPath, ct).ConfigureAwait(false);

            var intervalSec = input.Chunk.IntervalSec;
            var startSec = (int)Math.Floor(input.Chunk.StartSec);
            var endSec = (int)Math.Ceiling(input.Chunk.EndSec);

            var extractResult = await frameExtractor.ExtractFramesAsync(
                videoTempPath,
                intervalSec,
                framesTempDir,
                startSec,
                endSec).ConfigureAwait(false);

            var framePaths = extractResult.FramePaths;
            var uploadedKeys = await s3Storage.UploadFramesAsync(framesBucket, framesPrefix, framePaths, ct).ConfigureAwait(false);

            var manifestPrefix = output.ManifestPrefix?.TrimEnd('/') ?? "";
            if (!string.IsNullOrEmpty(manifestPrefix))
                manifestPrefix += "/";
            var manifestKey = manifestPrefix + "manifest.json";

            return new ChunkProcessorOutput(
                chunkId,
                ProcessingStatus.SUCCEEDED,
                uploadedKeys.Count,
                Manifest: new ManifestInfo(output.ManifestBucket, manifestKey));
        }
        catch (FileNotFoundException ex)
        {
            return new ChunkProcessorOutput(
                chunkId,
                ProcessingStatus.FAILED,
                0,
                Error: new ErrorInfo("VIDEO_NOT_FOUND", ex.Message, Retryable: false));
        }
        catch (InvalidOperationException ex)
        {
            return new ChunkProcessorOutput(
                chunkId,
                ProcessingStatus.FAILED,
                0,
                Error: new ErrorInfo("EXTRACTION_FAILED", ex.Message, Retryable: true));
        }
        finally
        {
            TryDeleteTempDir(baseTempDir);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "Best-effort cleanup: Directory.Delete pode falhar por bloqueio de arquivo; não é testável de forma determinística.")]
    private static void TryDeleteTempDir(string baseTempDir)
    {
        try
        {
            if (Directory.Exists(baseTempDir))
                Directory.Delete(baseTempDir, recursive: true);
        }
        catch
        {
            // Ignore
        }
    }
}
