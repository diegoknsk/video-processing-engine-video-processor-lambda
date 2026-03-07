# Subtask 05: Preservar CLI local e adicionar modo AWS/S3

## Descrição
Adicionar ao CLI existente um segundo modo de operação (`--mode aws`) que aceita os parâmetros de chunk/S3, constrói um `ChunkProcessorInput` e executa o `ProcessChunkUseCase` — permitindo testar o processamento real com S3 a partir da máquina local, sem precisar fazer deploy da Lambda. O modo local (sem `--mode`) deve continuar funcionando exatamente como hoje, sem nenhuma regressão.

## Passos de implementação

### 1. Preservar modo local sem alteração funcional
- O bloco de código atual do `Program.cs` (leitura de `--video`, `--interval`, `--output`, `--start`, `--end` e chamada ao `VideoFrameExtractor`) deve permanecer intacto ou ser encapsulado sem alterar o comportamento.
- Verificar que `dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --video <path> --interval 2 --output <path> --start 0 --end 10` funciona como antes após a mudança.

### 2. Detectar o modo de operação
No topo de `Program.cs`, verificar o argumento `--mode`:
```csharp
var mode = GetArg(args, "--mode") ?? "local";
```
Usar `if/else` simples para dividir o fluxo: modo `"local"` → fluxo atual; modo `"aws"` → novo fluxo.

### 3. Implementar parsing dos argumentos do modo AWS
Argumentos adicionais do modo AWS:
```
--video-id      <string>   ID do vídeo (videoId no contrato)
--chunk-id      <string>   ID do chunk (chunkId, ex: "chunk-001")
--interval      <int>      Intervalo em segundos entre frames
--start         <double>   Tempo de início do chunk em segundos
--end           <double>   Tempo de fim do chunk em segundos
--source-bucket <string>   Bucket S3 de origem
--source-key    <string>   Key S3 do vídeo de origem
--target-bucket <string>   Bucket S3 de destino dos frames
--target-prefix <string>   Prefixo S3 de destino (ex: "processed/video-abc/chunk-001/frames/")
```
Validar obrigatoriedade de cada argumento com mensagem de erro clara.

### 4. Construir ChunkProcessorInput e executar o use case
```csharp
var input = new ChunkProcessorInput(
    ContractVersion: "1.0",
    VideoId: videoId,
    Chunk: new ChunkInfo(chunkId, startSec, endSec, intervalSec),
    Source: new SourceInfo(sourceBucket, sourceKey),
    Output: new OutputConfig(
        ManifestBucket: targetBucket,
        ManifestPrefix: targetPrefix,
        FramesBucket: targetBucket,
        FramesPrefix: targetPrefix)
);
```
Instanciar `AmazonS3Client`, `S3VideoStorage`, `VideoFrameExtractor` e `ProcessChunkUseCase` diretamente (sem DI container no CLI — aceitável para modo de teste).

### 5. Exibir resultado no console
```
✓ Processamento concluído!
Status: SUCCEEDED
Frames: 31
Manifest S3: s3://bucket/processed/video-abc123/chunk-001/frames/manifest.json
```
Em caso de `Status = Failed`, exibir o erro e retornar exit code 1.

### 6. Atualizar `PrintUsage` para incluir exemplos de ambos os modos
Incluir no bloco de help:
```
Modo local:
  dotnet run -- --video <path> --interval <s> --output <pasta> [--start <s>] [--end <s>]

Modo AWS/S3:
  dotnet run -- --mode aws --video-id <id> --chunk-id <id> --interval <s> --start <s> --end <s>
               --source-bucket <b> --source-key <k> --target-bucket <b> --target-prefix <p>
```

## Notas técnicas
- `ChunkInfo` pode não ter `IntervalSec` ainda — confirmar com a Subtask 03 se foi adicionado; caso contrário, passar `intervalSec` como parâmetro separado.
- O CLI modo AWS usará as credentials AWS da máquina (`~/.aws/credentials` ou variáveis de ambiente `AWS_ACCESS_KEY_ID`/`AWS_SECRET_ACCESS_KEY`/`AWS_DEFAULT_REGION`). Documentar isso na Subtask 07.
- Não usar DI container no CLI — instanciação direta é suficiente e mantém o CLI simples.

## Formas de teste
- Executar `dotnet run -- --video <path> --interval 2 --output <pasta> --start 0 --end 10` e confirmar que o resultado é idêntico ao atual (teste de regressão modo local).
- Executar `dotnet run -- --mode aws ...` com bucket e vídeo reais na AWS e confirmar frames no bucket de destino.
- Executar sem argumentos e confirmar que o help exibe ambos os modos.

## Critérios de aceite da subtask
- [ ] Modo local (`--mode local` ou sem `--mode`) funciona exatamente como antes — sem nenhuma regressão.
- [ ] Modo AWS (`--mode aws`) aceita os 9 argumentos obrigatórios e os valida com mensagens de erro claras quando ausentes.
- [ ] Modo AWS constrói `ChunkProcessorInput` e executa `ProcessChunkUseCase` (mesmo use case da Lambda).
- [ ] Em modo AWS com credenciais e buckets válidos, os frames aparecem no bucket S3 de destino após execução.
- [ ] Console exibe resultado com `Status`, `FramesCount` e S3 key do destino; exit code 0 em sucesso, 1 em falha.
- [ ] Help (`--help` ou execução sem args obrigatórios) exibe exemplos de ambos os modos.
