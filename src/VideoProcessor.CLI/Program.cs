using VideoProcessor.Application.Services;

static string? GetArg(string[] args, string name)
{
    var i = Array.IndexOf(args, name);
    if (i < 0 || i >= args.Length - 1) return null;
    return args[i + 1];
}

static void PrintUsage()
{
    Console.WriteLine("Uso: dotnet run -- --video <caminho> --interval <segundos> --output <pasta> [--start <s>] [--end <s>]");
    Console.WriteLine("  --video    Caminho do arquivo de vídeo (ex: sample.mp4)");
    Console.WriteLine("  --interval Intervalo em segundos entre frames (ex: 20)");
    Console.WriteLine("  --output   Pasta de saída dos frames (ex: output/frames)");
    Console.WriteLine("  --start    (Opcional) Tempo de início do trecho em segundos (ex: 0 para primeiro minuto: --start 0 --end 59)");
    Console.WriteLine("  --end      (Opcional) Tempo de fim do trecho em segundos (ex: 59 para processar 0s a 59s)");
}

var video = GetArg(args, "--video");
var intervalStr = GetArg(args, "--interval");
var output = GetArg(args, "--output");

if (string.IsNullOrWhiteSpace(video) || string.IsNullOrWhiteSpace(intervalStr) || string.IsNullOrWhiteSpace(output))
{
    Console.Error.WriteLine("Erro: argumentos obrigatórios --video, --interval e --output.");
    PrintUsage();
    return 1;
}

if (!int.TryParse(intervalStr, out var interval) || interval < 1)
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
var startStr = GetArg(args, "--start");
var endStr = GetArg(args, "--end");
if (startStr != null)
{
    if (!int.TryParse(startStr, out var startVal) || startVal < 0)
    {
        Console.Error.WriteLine("Erro: --start deve ser um número >= 0.");
        return 1;
    }
    startSeconds = startVal;
}
if (endStr != null)
{
    if (!int.TryParse(endStr, out var endVal) || endVal < 0)
    {
        Console.Error.WriteLine("Erro: --end deve ser um número >= 0.");
        return 1;
    }
    endSeconds = endVal;
}

Console.WriteLine($"Processando vídeo: {Path.GetFileName(videoPath)}");
Console.WriteLine($"Intervalo: {interval}s");
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

var extractor = new VideoFrameExtractor(progress);
var result = await extractor.ExtractFramesAsync(videoPath, interval, output, startSeconds, endSeconds);

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine("✓ Processamento concluído!");
Console.WriteLine($"Total de frames: {result.TotalFrames}");
Console.WriteLine($"Pasta de saída: {Path.GetFullPath(output)}/");
Console.WriteLine($"Duração do vídeo: {result.VideoDuration:hh\\:mm\\:ss}");
Console.WriteLine($"Tempo de processamento: {result.ProcessingDuration.TotalSeconds:F1}s");

return 0;
