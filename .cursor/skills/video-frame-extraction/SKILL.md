---
name: video-frame-extraction
description: Extract frames from videos at specified intervals using FFmpeg in .NET Lambda functions. Use when processing videos, capturing frames, generating thumbnails, or working with video frame extraction on AWS Lambda.
---

# Video Frame Extraction for .NET Lambda

Especialista em extrair frames de vídeos usando FFmpeg em funções AWS Lambda com .NET. Esta skill cobre configuração do FFmpeg, extração de frames em intervalos parametrizáveis, e integração com S3.

## Quick Start

Para extrair frames de um vídeo em Lambda:

1. **Configurar FFmpeg Layer** no Lambda
2. **Baixar vídeo** (do S3 ou outra fonte)
3. **Extrair frames** usando Xabe.FFmpeg
4. **Salvar frames** (S3 recomendado)
5. **Limpar arquivos temporários** em /tmp

## Configuração FFmpeg no Lambda

### Lambda Layer

FFmpeg deve ser adicionado como Lambda Layer em `/opt/ffmpeg/`:

```csharp
private void InitializeFFmpeg(ILambdaContext context)
{
    var possiblePaths = new[]
    {
        ("/opt/ffmpeg/ffmpeg", "/opt/ffmpeg/ffprobe"),
        ("/opt/bin/ffmpeg", "/opt/bin/ffprobe"),
        ("/var/task/ffmpeg", "/var/task/ffprobe")
    };

    foreach (var (ffmpeg, ffprobe) in possiblePaths)
    {
        if (File.Exists(ffmpeg) && File.Exists(ffprobe))
        {
            var ffmpegDir = Path.GetDirectoryName(ffmpeg);
            FFmpeg.SetExecutablesPath(ffmpegDir);
            context.Logger.LogInformation($"FFmpeg configurado em: {ffmpegDir}");
            return;
        }
    }
    
    throw new Exception("FFmpeg não encontrado no Lambda Layer");
}
```

### Pacotes NuGet Necessários

```xml
<PackageReference Include="Xabe.FFmpeg" Version="5.2.6" />
<PackageReference Include="AWSSDK.S3" Version="3.7.300.2" />
<PackageReference Include="Amazon.Lambda.Core" Version="2.2.0" />
```

## Extração de Frames

### Exemplo Básico - Intervalo Parametrizável

```csharp
public async Task<List<string>> ExtractFrames(
    string videoPath, 
    int intervalSeconds, 
    string outputFolder,
    ILambdaContext context)
{
    var framesPaths = new List<string>();
    
    // Obter informações do vídeo
    var mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
    var duration = mediaInfo.Duration;
    
    context.Logger.LogInformation(
        $"Vídeo: {duration.TotalSeconds}s, Intervalo: {intervalSeconds}s");
    
    // Calcular quantidade de frames
    var frameCount = (int)(duration.TotalSeconds / intervalSeconds);
    
    // Extrair frames em paralelo
    var tasks = new List<Task>();
    for (var i = 0; i < frameCount; i++)
    {
        var currentTime = TimeSpan.FromSeconds(i * intervalSeconds);
        var outputPath = Path.Combine(outputFolder, $"frame_{i:D4}_{currentTime.TotalSeconds:F0}s.jpg");
        
        var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
            videoPath, 
            outputPath, 
            currentTime);
        
        tasks.Add(conversion.Start());
        framesPaths.Add(outputPath);
    }
    
    await Task.WhenAll(tasks);
    context.Logger.LogInformation($"{frameCount} frames extraídos com sucesso");
    
    return framesPaths;
}
```

### Extração com Qualidade Customizada

```csharp
public async Task<string> ExtractFrameWithQuality(
    string videoPath,
    TimeSpan timestamp,
    string outputPath,
    int quality = 85) // 1-100
{
    var mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
    
    var conversion = FFmpeg.Conversions.New()
        .AddParameter($"-ss {timestamp.TotalSeconds}")
        .AddParameter($"-i \"{videoPath}\"")
        .AddParameter("-vframes 1")
        .AddParameter($"-q:v {(100 - quality) / 4}") // Converter qualidade para escala FFmpeg
        .SetOutput(outputPath);
    
    await conversion.Start();
    return outputPath;
}
```

### Extração com Redimensionamento

```csharp
public async Task<string> ExtractFrameResized(
    string videoPath,
    TimeSpan timestamp,
    string outputPath,
    int width = 1280,
    int height = 720)
{
    var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
        videoPath, 
        outputPath, 
        timestamp);
    
    conversion.AddParameter($"-vf scale={width}:{height}");
    await conversion.Start();
    
    return outputPath;
}
```

## Integração com S3

### Download de Vídeo do S3

```csharp
private async Task<string> DownloadVideoFromS3(
    string bucketName, 
    string videoKey,
    IAmazonS3 s3Client)
{
    var localPath = Path.Combine("/tmp", Path.GetFileName(videoKey));
    
    var response = await s3Client.GetObjectAsync(new GetObjectRequest
    {
        BucketName = bucketName,
        Key = videoKey
    });
    
    using var fileStream = File.Create(localPath);
    await response.ResponseStream.CopyToAsync(fileStream);
    
    return localPath;
}
```

### Upload de Frames para S3

```csharp
private async Task<List<string>> UploadFramesToS3(
    string bucketName,
    List<string> framePaths,
    string videoId,
    IAmazonS3 s3Client,
    ILambdaContext context)
{
    var s3Keys = new List<string>();
    var tasks = new List<Task<string>>();
    
    foreach (var framePath in framePaths)
    {
        tasks.Add(UploadSingleFrame(bucketName, framePath, videoId, s3Client));
    }
    
    s3Keys = (await Task.WhenAll(tasks)).ToList();
    
    context.Logger.LogInformation($"{s3Keys.Count} frames enviados para S3");
    return s3Keys;
}

private async Task<string> UploadSingleFrame(
    string bucketName,
    string framePath,
    string videoId,
    IAmazonS3 s3Client)
{
    var fileName = Path.GetFileName(framePath);
    var s3Key = $"frames/{videoId}/{fileName}";
    
    await s3Client.PutObjectAsync(new PutObjectRequest
    {
        BucketName = bucketName,
        Key = s3Key,
        FilePath = framePath
    });
    
    return s3Key;
}
```

### Upload como ZIP (Otimizado)

```csharp
private async Task<string> UploadFramesAsZip(
    string bucketName,
    string framesFolder,
    string videoId,
    IAmazonS3 s3Client,
    ILambdaContext context)
{
    var zipPath = Path.Combine("/tmp", $"{videoId}_frames.zip");
    
    // Criar ZIP
    ZipFile.CreateFromDirectory(framesFolder, zipPath);
    context.Logger.LogInformation($"ZIP criado: {new FileInfo(zipPath).Length / 1024}KB");
    
    // Upload para S3
    var s3Key = $"frames/{videoId}_frames.zip";
    await s3Client.PutObjectAsync(new PutObjectRequest
    {
        BucketName = bucketName,
        Key = s3Key,
        FilePath = zipPath
    });
    
    // Limpar ZIP
    File.Delete(zipPath);
    
    return s3Key;
}
```

## Lambda Handler Completo

```csharp
public class VideoFrameExtractorFunction
{
    private readonly IAmazonS3 _s3Client;
    private const string TEMP_DIR = "/tmp";
    
    public VideoFrameExtractorFunction()
    {
        _s3Client = new AmazonS3Client();
    }
    
    public async Task<FrameExtractionResponse> FunctionHandler(
        FrameExtractionRequest request, 
        ILambdaContext context)
    {
        string localVideoPath = null;
        string framesFolder = null;
        
        try
        {
            // Inicializar FFmpeg
            InitializeFFmpeg(context);
            
            // Download do vídeo
            context.Logger.LogInformation($"Baixando vídeo: {request.VideoKey}");
            localVideoPath = await DownloadVideoFromS3(
                request.BucketName, 
                request.VideoKey, 
                _s3Client);
            
            // Criar pasta para frames
            framesFolder = Path.Combine(TEMP_DIR, "frames");
            Directory.CreateDirectory(framesFolder);
            
            // Extrair frames
            context.Logger.LogInformation(
                $"Extraindo frames a cada {request.IntervalSeconds}s");
            
            var framePaths = await ExtractFrames(
                localVideoPath,
                request.IntervalSeconds,
                framesFolder,
                context);
            
            // Upload para S3
            string outputLocation;
            if (request.UploadAsZip)
            {
                outputLocation = await UploadFramesAsZip(
                    request.BucketName,
                    framesFolder,
                    request.VideoId,
                    _s3Client,
                    context);
            }
            else
            {
                var s3Keys = await UploadFramesToS3(
                    request.BucketName,
                    framePaths,
                    request.VideoId,
                    _s3Client,
                    context);
                outputLocation = string.Join(", ", s3Keys);
            }
            
            return new FrameExtractionResponse
            {
                Success = true,
                VideoId = request.VideoId,
                FrameCount = framePaths.Count,
                OutputLocation = outputLocation
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro: {ex.Message}");
            throw;
        }
        finally
        {
            // Limpeza
            CleanupTempFiles(localVideoPath, framesFolder, context);
        }
    }
}

// Modelos
public class FrameExtractionRequest
{
    public string BucketName { get; set; }
    public string VideoKey { get; set; }
    public string VideoId { get; set; }
    public int IntervalSeconds { get; set; } = 20;
    public bool UploadAsZip { get; set; } = true;
}

public class FrameExtractionResponse
{
    public bool Success { get; set; }
    public string VideoId { get; set; }
    public int FrameCount { get; set; }
    public string OutputLocation { get; set; }
}
```

## Limpeza de Arquivos Temporários

```csharp
private void CleanupTempFiles(
    string videoPath, 
    string framesFolder,
    ILambdaContext context)
{
    try
    {
        if (videoPath != null && File.Exists(videoPath))
        {
            File.Delete(videoPath);
            context.Logger.LogInformation($"Vídeo temporário removido");
        }
        
        if (framesFolder != null && Directory.Exists(framesFolder))
        {
            Directory.Delete(framesFolder, true);
            context.Logger.LogInformation($"Pasta de frames removida");
        }
    }
    catch (Exception ex)
    {
        context.Logger.LogWarning($"Erro ao limpar arquivos: {ex.Message}");
    }
}
```

## Boas Práticas Lambda

### 1. Gerenciamento de Memória

- **Vídeos grandes**: Aumente memória Lambda (2048-10240 MB)
- **Limite /tmp**: 512MB - 10GB (configurável)
- **Sempre limpar**: Use `finally` para deletar arquivos temporários

### 2. Timeout

Configure timeout adequado baseado no tamanho do vídeo:
- Vídeos até 5 min: 60s timeout
- Vídeos até 30 min: 300s timeout
- Vídeos maiores: 900s timeout (máximo)

### 3. Processamento Paralelo

```csharp
// BOM - Paralelo
var tasks = frames.Select(f => ProcessFrame(f));
await Task.WhenAll(tasks);

// EVITAR - Sequencial
foreach (var frame in frames)
{
    await ProcessFrame(frame);
}
```

### 4. Logs Estruturados

```csharp
context.Logger.LogInformation($"[{videoId}] Iniciando extração");
context.Logger.LogInformation($"[{videoId}] Frames: {count}, Tamanho: {size}MB");
context.Logger.LogInformation($"[{videoId}] Concluído em {elapsed}ms");
```

## Configuração Lambda

### Variáveis de Ambiente

```env
BUCKET_NAME=seu-bucket-s3
TEMP_DIR=/tmp
DEFAULT_INTERVAL_SECONDS=20
MAX_VIDEO_SIZE_MB=500
```

### IAM Permissions

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:PutObject"
      ],
      "Resource": "arn:aws:s3:::seu-bucket/*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ],
      "Resource": "arn:aws:logs:*:*:*"
    }
  ]
}
```

### Estrutura do Projeto

```
VideoFrameExtractor/
├── Function.cs                    # Handler principal
├── Services/
│   ├── FFmpegService.cs          # Lógica de extração
│   └── S3Service.cs              # Upload/download S3
├── Models/
│   ├── FrameExtractionRequest.cs
│   └── FrameExtractionResponse.cs
└── VideoFrameExtractor.csproj
```

## Troubleshooting

### FFmpeg não encontrado

```csharp
// Verificar se Layer está configurado
var layerPath = "/opt/ffmpeg/ffmpeg";
if (!File.Exists(layerPath))
{
    context.Logger.LogError("FFmpeg Layer não configurado!");
    // Listar conteúdo de /opt para debug
    var optFiles = Directory.GetFiles("/opt", "*", SearchOption.AllDirectories);
    context.Logger.LogInformation($"Arquivos em /opt: {string.Join(", ", optFiles)}");
}
```

### Erro de memória em /tmp

```csharp
// Verificar espaço disponível
var tmpInfo = new DriveInfo("/tmp");
var availableMB = tmpInfo.AvailableFreeSpace / (1024 * 1024);
context.Logger.LogInformation($"Espaço disponível em /tmp: {availableMB}MB");

if (availableMB < 100)
{
    throw new Exception("Espaço insuficiente em /tmp");
}
```

### Timeout de processamento

```csharp
// Processar em chunks menores
var chunkSize = 50; // Processar 50 frames por vez
for (int i = 0; i < framePaths.Count; i += chunkSize)
{
    var chunk = framePaths.Skip(i).Take(chunkSize);
    await UploadFramesToS3(bucketName, chunk.ToList(), videoId, s3Client, context);
    context.Logger.LogInformation($"Chunk {i / chunkSize + 1} enviado");
}
```

## Otimizações

### 1. Reduzir Qualidade para Thumbnails

```csharp
conversion.AddParameter("-q:v 5"); // Qualidade média (2=melhor, 31=pior)
conversion.AddParameter("-vf scale=640:360"); // Resolução menor
```

### 2. Limitar Taxa de Processamento

```csharp
// Processar no máximo N frames por segundo
var semaphore = new SemaphoreSlim(5); // 5 frames simultâneos
var tasks = frameTasks.Select(async task =>
{
    await semaphore.WaitAsync();
    try
    {
        return await task;
    }
    finally
    {
        semaphore.Release();
    }
});
```

### 3. Cache de MediaInfo

```csharp
private static readonly Dictionary<string, IMediaInfo> _mediaInfoCache = new();

private async Task<IMediaInfo> GetMediaInfoCached(string videoPath)
{
    if (!_mediaInfoCache.ContainsKey(videoPath))
    {
        _mediaInfoCache[videoPath] = await FFmpeg.GetMediaInfo(videoPath);
    }
    return _mediaInfoCache[videoPath];
}
```

## Referências Rápidas

### Formatos de Vídeo Suportados
- MP4, AVI, MOV, MKV, WebM, FLV

### Formatos de Saída
- JPG (recomendado para frames)
- PNG (maior qualidade, maior tamanho)
- WebP (melhor compressão)

### Comandos FFmpeg Úteis

```csharp
// Extrair frame específico
"-ss 00:01:30 -i video.mp4 -vframes 1 frame.jpg"

// Extrair a cada N segundos
"-i video.mp4 -vf fps=1/20 frame_%04d.jpg"

// Com qualidade
"-i video.mp4 -q:v 2 -vframes 1 frame.jpg"

// Com resize
"-i video.mp4 -vf scale=1280:720 -vframes 1 frame.jpg"
```
