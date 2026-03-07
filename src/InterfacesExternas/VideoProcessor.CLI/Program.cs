using Amazon.S3;
using VideoProcessor.Application.Services;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Infra.S3;

static string? GetArg(string[] args, string name)
{
    var i = Array.IndexOf(args, name);
    if (i < 0 || i >= args.Length - 1) return null;
    return args[i + 1];
}

static void PrintUsage()
{
    Console.WriteLine("Modo local:");
    Console.WriteLine("  dotnet run -- --video <path> --interval <s> --output <pasta> [--start <s>] [--end <s>]");
    Console.WriteLine();
    Console.WriteLine("Modo AWS/S3:");
    Console.WriteLine("  dotnet run -- --mode aws --video-id <id> --chunk-id <id> --interval <s> --start <s> --end <s>");
    Console.WriteLine("    --source-bucket <b> --source-key <k> --target-bucket <b> --target-prefix <p>");
    Console.WriteLine();
    Console.WriteLine("Modo local: --video, --interval, --output obrigatórios. --start e --end opcionais.");
    Console.WriteLine("Modo AWS: --mode aws e todos os argumentos acima obrigatórios.");
}

var mode = GetArg(args, "--mode") ?? "local";

if (string.Equals(mode, "aws", StringComparison.OrdinalIgnoreCase))
{
    // --- Modo AWS: ChunkProcessorInput + ProcessChunkUseCase ---
    var videoId = GetArg(args, "--video-id");
    var chunkId = GetArg(args, "--chunk-id");
    var intervalStr = GetArg(args, "--interval");
    var startStr = GetArg(args, "--start");
    var endStr = GetArg(args, "--end");
    var sourceBucket = GetArg(args, "--source-bucket");
    var sourceKey = GetArg(args, "--source-key");
    var targetBucket = GetArg(args, "--target-bucket");
    var targetPrefix = GetArg(args, "--target-prefix");

    var required = new[] { ("--video-id", videoId), ("--chunk-id", chunkId), ("--interval", intervalStr),
        ("--start", startStr), ("--end", endStr), ("--source-bucket", sourceBucket), ("--source-key", sourceKey),
        ("--target-bucket", targetBucket), ("--target-prefix", targetPrefix) };

    var missing = required.Where(t => string.IsNullOrWhiteSpace(t.Item2)).Select(t => t.Item1).ToList();
    if (missing.Count > 0)
    {
        Console.Error.WriteLine($"Erro: argumentos obrigatórios ausentes: {string.Join(", ", missing)}");
        PrintUsage();
        return 1;
    }

    if (!int.TryParse(intervalStr!.Trim(), out var interval) || interval < 1)
    {
        Console.Error.WriteLine("Erro: --interval deve ser um número >= 1.");
        return 1;
    }
    if (!double.TryParse(startStr!.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var startSec))
    {
        Console.Error.WriteLine("Erro: --start deve ser um número.");
        return 1;
    }
    if (!double.TryParse(endStr!.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var endSec))
    {
        Console.Error.WriteLine("Erro: --end deve ser um número.");
        return 1;
    }

    var prefixNorm = targetPrefix!.TrimEnd('/');
    if (!string.IsNullOrEmpty(prefixNorm))
        prefixNorm += "/";

    var input = new ChunkProcessorInput(
        ContractVersion: "1.0",
        VideoId: videoId!,
        Chunk: new ChunkInfo(chunkId!, startSec, endSec, interval),
        Source: new SourceInfo(sourceBucket!, sourceKey!),
        Output: new OutputConfig(
            ManifestBucket: targetBucket!,
            ManifestPrefix: prefixNorm,
            FramesBucket: targetBucket,
            FramesPrefix: prefixNorm));

    await FFmpegSetup.EnsureFFmpegInstalledAsync();
    var s3Client = new AmazonS3Client();
    var s3Storage = new S3VideoStorage(s3Client);
    var extractor = new VideoFrameExtractor();
    var useCase = new ProcessChunkUseCase(extractor, s3Storage);

    Console.WriteLine($"Processando chunk no modo AWS. VideoId={videoId} ChunkId={chunkId}");
    var output = await useCase.ExecuteAsync(input);

    Console.WriteLine();
    Console.WriteLine("---");
    if (output.Status == ProcessingStatus.SUCCEEDED)
    {
        Console.WriteLine("✓ Processamento concluído!");
        Console.WriteLine($"Status: {output.Status}");
        Console.WriteLine($"Frames: {output.FramesCount}");
        if (output.Manifest != null)
            Console.WriteLine($"Manifest S3: s3://{output.Manifest.Bucket}/{output.Manifest.Key}");
        return 0;
    }
    Console.Error.WriteLine($"Falha: {output.Error?.Message ?? "Erro desconhecido"}");
    return 1;
}

// --- Modo local: fluxo original (VideoFrameExtractor direto) ---
var video = GetArg(args, "--video");
var intervalStrLocal = GetArg(args, "--interval");
var outputPath = GetArg(args, "--output");

if (string.IsNullOrWhiteSpace(video) || string.IsNullOrWhiteSpace(intervalStrLocal) || string.IsNullOrWhiteSpace(outputPath))
{
    Console.Error.WriteLine("Erro: argumentos obrigatórios --video, --interval e --output.");
    PrintUsage();
    return 1;
}

if (!int.TryParse(intervalStrLocal, out var intervalLocal) || intervalLocal < 1)
{
    Console.Error.WriteLine("Erro: --interval deve ser um número >= 1.");
    return 1;
}

var videoPath = Path.GetFullPath(video);
if (!File.Exists(videoPath))
{
    Console.Error.WriteLine($"Erro: arquivo de vídeo não encontrado: {videoPath}");
    return 1;
}

int? startSeconds = null;
int? endSeconds = null;
var startStrLocal = GetArg(args, "--start");
var endStrLocal = GetArg(args, "--end");
if (startStrLocal != null)
{
    if (!int.TryParse(startStrLocal, out var startVal) || startVal < 0)
    {
        Console.Error.WriteLine("Erro: --start deve ser um número >= 0.");
        return 1;
    }
    startSeconds = startVal;
}
if (endStrLocal != null)
{
    if (!int.TryParse(endStrLocal, out var endVal) || endVal < 0)
    {
        Console.Error.WriteLine("Erro: --end deve ser um número >= 0.");
        return 1;
    }
    endSeconds = endVal;
}

Console.WriteLine($"Processando vídeo: {Path.GetFileName(videoPath)}");
Console.WriteLine($"Intervalo: {intervalLocal}s");
if (startSeconds.HasValue || endSeconds.HasValue)
    Console.WriteLine($"Trecho: {startSeconds ?? 0}s a {endSeconds?.ToString() ?? "fim"}");
Console.WriteLine();

await FFmpegSetup.EnsureFFmpegInstalledAsync();
Console.WriteLine($"FFmpeg disponível em: {FFmpegSetup.GetFFmpegPath()}");
Console.WriteLine();

var progress = new Progress<(int Current, int Total)>(p =>
{
    var percent = p.Total > 0 ? (int)(100.0 * p.Current / p.Total) : 0;
    Console.Write($"\rExtraindo frames... {p.Current}/{p.Total} concluídos ({percent}%)");
});

var extractorLocal = new VideoFrameExtractor(progress);
var result = await extractorLocal.ExtractFramesAsync(videoPath, intervalLocal, outputPath, startSeconds, endSeconds);

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine("✓ Processamento concluído!");
Console.WriteLine($"Total de frames: {result.TotalFrames}");
Console.WriteLine($"Pasta de saída: {Path.GetFullPath(outputPath)}/");
Console.WriteLine($"Duração do vídeo: {result.VideoDuration:hh\\:mm\\:ss}");
Console.WriteLine($"Tempo de processamento: {result.ProcessingDuration.TotalSeconds:F1}s");

return 0;
