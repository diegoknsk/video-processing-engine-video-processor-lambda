# Subtask 04: Adaptar Lambda Function.cs para processamento real

## Descrição
Substituir o handler mockado ("Hello World") da Lambda pelo processamento real, conectando o `ProcessChunkUseCase` ao handler do Lambda. O `Function.cs` deve: configurar DI uma única vez (cold start), desserializar o input como `ChunkProcessorInput`, chamar `ProcessChunkUseCase.ExecuteAsync`, serializar e retornar o `ChunkProcessorOutput`. Também é necessário configurar corretamente o caminho do FFmpeg para o ambiente Lambda (`/opt/bin/ffmpeg` via Lambda Layer).

## Passos de implementação

### 1. Configurar DI no construtor de `Function`
Substituir o corpo atual de `Function.cs` por uma classe com construtor que configura `ServiceCollection`:
```csharp
public class Function
{
    private readonly ProcessChunkUseCase _useCase;

    public Function()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client());
        services.AddSingleton<IS3VideoStorage, S3VideoStorage>();
        services.AddSingleton<IVideoFrameExtractor>(_ => new VideoFrameExtractor());
        services.AddSingleton<ProcessChunkUseCase>();
        var sp = services.BuildServiceProvider();
        _useCase = sp.GetRequiredService<ProcessChunkUseCase>();
    }
    // ...
}
```

### 2. Configurar FFmpeg para Lambda Layer
Antes de registrar `IVideoFrameExtractor`, configurar o path do FFmpeg:
```csharp
// Lambda Layer publica FFmpeg em /opt/bin/ffmpeg
var ffmpegPath = Environment.GetEnvironmentVariable("FFMPEG_PATH") ?? "/opt/bin";
if (Directory.Exists(ffmpegPath))
    FFmpeg.SetExecutablesPath(ffmpegPath);
```
Usar a variável de ambiente `FFMPEG_PATH` para permitir override sem redeployar (útil para testes).

### 3. Atualizar a assinatura do handler
Trocar a assinatura atual (`string input`) por:
```csharp
public async Task<string> FunctionHandler(ChunkProcessorInput input, ILambdaContext context)
```
O `DefaultLambdaJsonSerializer` já existente fará a desserialização automaticamente.

### 4. Implementar o handler
```csharp
public async Task<string> FunctionHandler(ChunkProcessorInput input, ILambdaContext context)
{
    context.Logger.LogInformation("Iniciando processamento. VideoId={VideoId} ChunkId={ChunkId}",
        input.VideoId, input.Chunk.ChunkId);

    var output = await _useCase.ExecuteAsync(input, context.GetCancellationToken());

    context.Logger.LogInformation("Processamento concluído. Status={Status} Frames={Frames}",
        output.Status, output.FramesCount);

    return JsonSerializer.Serialize(output, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });
}
```

### 5. Verificar `CancellationToken` da Lambda
Usar `context.GetCancellationToken()` se disponível no SDK, ou `CancellationToken.None` se não existir na versão do SDK usada. Documentar.

### 6. Verificar Lambda Layer de FFmpeg
Confirmar (ou documentar como pré-condição) que a Lambda possui uma Layer com FFmpeg binário publicado em `/opt/bin/ffmpeg`. Se a Layer não existir ainda, documentar os passos de criação na subtask de documentação (Subtask 07).

## Notas de configuração da Lambda
- **Timeout recomendado:** ≥ 5 minutos (300s) para chunks de até 60s com vídeos HD.
- **Memória recomendada:** ≥ 2048 MB para processamento confortável.
- **Ephemeral storage (`/tmp`):** configurar ≥ 1 GB para chunks de vídeo HD.
- **Variável de ambiente `FFMPEG_PATH`:** definir como `/opt/bin` (ou o path da Layer).
- **IAM Role:** permissões `s3:GetObject` no bucket de origem e `s3:PutObject` no bucket de destino.

## Formas de teste
- Invocar a Lambda localmente usando o AWS .NET Lambda Mock Test Tool (se disponível) ou via `dotnet lambda invoke-function` após deploy.
- Testar via Console AWS com o payload de exemplo da Subtask 07.
- Verificar logs no CloudWatch: devem aparecer as linhas de início e conclusão com `VideoId`, `ChunkId`, `Status` e `FramesCount`.
- Executar `dotnet test` e confirmar que os testes do projeto Lambda (se existirem) passam.

## Critérios de aceite da subtask
- [ ] `Function.cs` não contém mais o handler Hello World; o handler recebe `ChunkProcessorInput` e retorna string JSON serializada de `ChunkProcessorOutput`.
- [ ] DI configurado via `ServiceCollection` no construtor de `Function` (cold start único).
- [ ] FFmpeg path configurado a partir de variável de ambiente `FFMPEG_PATH` com fallback para `/opt/bin`.
- [ ] Handler loga início e fim com `VideoId`, `ChunkId`, `Status` e `FramesCount`.
- [ ] Falhas controladas (FileNotFoundException, InvalidOperationException) não propagam exceção — são capturadas pelo use case e retornadas como `ChunkProcessorOutput` com `Status = Failed`.
- [ ] `dotnet build` da Lambda conclui sem erros.
- [ ] Ao invocar a Lambda com payload válido e bucket S3 acessível, frames aparecem no bucket de destino e o retorno JSON tem `status: "succeeded"`.
