# Subtask 03: Criar ProcessChunkUseCase na Application

## Descrição
Criar o use case central `ProcessChunkUseCase` na camada Application, responsável por orquestrar o fluxo completo de processamento de um chunk de vídeo: download do vídeo do S3 para `/tmp`, extração dos frames via `IVideoFrameExtractor`, upload dos frames para S3 via `IS3VideoStorage`, limpeza dos arquivos temporários e retorno de `ChunkProcessorOutput`. Este use case é a unidade de lógica reutilizada tanto pela Lambda quanto pelo CLI modo AWS.

## Passos de implementação
1. Criar `src/Core/VideoProcessor.Application/UseCases/ProcessChunkUseCase.cs` no namespace `VideoProcessor.Application.UseCases`.

2. Construtor via DI com dois ports injetados:
   ```csharp
   public ProcessChunkUseCase(
       IVideoFrameExtractor frameExtractor,
       IS3VideoStorage s3Storage)
   ```

3. Implementar o método principal:
   ```csharp
   public async Task<ChunkProcessorOutput> ExecuteAsync(
       ChunkProcessorInput input,
       CancellationToken ct = default)
   ```

4. Fluxo interno do `ExecuteAsync`:
   - **Validação básica:** confirmar que `input` não é null e que `input.Source.Bucket`, `input.Source.Key`, `input.Output.FramesBucket` e `input.Output.FramesPrefix` estão preenchidos.
   - **Paths temporários:** usar `/tmp/{input.VideoId}/{input.Chunk.ChunkId}/` como pasta base temporária. Criar subpastas `video/` (para o arquivo baixado) e `frames/` (para os frames extraídos).
   - **Download:** chamar `IS3VideoStorage.DownloadToTempAsync(source.Bucket, source.Key, videoTempPath, ct)`.
   - **Extração:** chamar `IVideoFrameExtractor.ExtractFramesAsync(videoTempPath, intervalSeconds, framesFolder, startSec, endSec)`. Os parâmetros `intervalSeconds`, `startSec` e `endSec` devem vir de `input.Chunk` (startSec e endSec como `int`, obtidos via cast de `double`).
   - **Upload:** chamar `IS3VideoStorage.UploadFramesAsync(output.FramesBucket, output.FramesPrefix, framePaths, ct)`.
   - **Cleanup:** remover a pasta temporária base (`/tmp/{videoId}/{chunkId}/`) após o upload — independentemente de sucesso ou falha (usar `try/finally`).
   - **Retorno (sucesso):** `new ChunkProcessorOutput(input.Chunk.ChunkId, ProcessingStatus.Succeeded, framesCount, manifest: new ManifestInfo(output.FramesBucket, $"{output.FramesPrefix}manifest.json"))`.
   - **Retorno (falha controlada):** capturar exceções conhecidas (`FileNotFoundException`, `InvalidOperationException`) e retornar `new ChunkProcessorOutput(input.Chunk.ChunkId, ProcessingStatus.Failed, 0, Error: new ErrorInfo(...))`. Exceções inesperadas devem propagar normalmente.

5. Definir `intervalSeconds` como parâmetro adicional em `ExecuteAsync` ou como campo no `ChunkProcessorInput`. Verificar se `ChunkProcessorInput` já possui ou adicionar `int IntervalSec` ao record em Domain (confirmar com a equipe antes; se `ChunkInfo` não possui, adicionar `int IntervalSec` ao record `ChunkInfo` ou ao `ChunkProcessorInput` diretamente).

> **Nota de arquitetura:** O use case não conhece Lambda, AWS CLI, nem qualquer detalhe de interface externa. Ele conhece apenas os dois ports (`IVideoFrameExtractor`, `IS3VideoStorage`) e os modelos do Domain. Isso garante testabilidade e reaproveitamento.

## Formas de teste
- Teste unitário (Subtask 06): mockar `IVideoFrameExtractor` e `IS3VideoStorage`, invocar `ExecuteAsync` com input válido e verificar que o output tem `Status = Succeeded` e `FramesCount` correto.
- Teste unitário (Subtask 06): simular falha no `DownloadToTempAsync` (FileNotFoundException) e verificar que o output retornado tem `Status = Failed` e `Error` preenchido.
- Compilar `VideoProcessor.Application` isoladamente e confirmar zero dependências de AWS.
- Revisão: confirmar que `ProcessChunkUseCase` não contém nenhum `using Amazon.*`.

## Critérios de aceite da subtask
- [ ] `ProcessChunkUseCase` criado em `src/Core/VideoProcessor.Application/UseCases/` com construtor recebendo `IVideoFrameExtractor` e `IS3VideoStorage`.
- [ ] Método `ExecuteAsync(ChunkProcessorInput, CancellationToken)` implementado com o fluxo completo: validação → download → extração → upload → cleanup → retorno.
- [ ] Cleanup de arquivos temporários executado em `try/finally` (executa mesmo em caso de falha no upload ou extração).
- [ ] Falhas conhecidas (`FileNotFoundException`, `InvalidOperationException`) resultam em `ChunkProcessorOutput` com `Status = Failed` e `Error` preenchido — sem lançar exceção para o chamador.
- [ ] Nenhuma referência a `Amazon.*`, `System.CommandLine`, ou qualquer detalhe de interface externa no arquivo.
- [ ] `dotnet build` da camada Application conclui sem erros.
- [ ] `ChunkProcessorInput` possui campo de intervalo (`IntervalSec` ou equivalente) para que o use case possa chamá-lo sem parâmetro extra — confirmado e ajustado se necessário.
