# Subtask 04: Escrever manifest.json e done.json Mockados no S3

## Descrição
Completar `ProcessChunkUseCase` para, após processamento mockado, escrever `manifest.json` com estrutura básica e `done.json` como marker no S3, e retornar SUCCEEDED.

## Passos de Implementação
1. No `ProcessChunkUseCase.ExecuteAsync`, após verificação de idempotência (quando `done.json` não existe):
   ```csharp
   // Processar chunk (mock)
   logger.LogInformation("Processing chunk {ChunkId} from {StartSec}s to {EndSec}s", 
       input.Chunk.ChunkId, input.Chunk.StartSec, input.Chunk.EndSec);
   
   // TODO: extrair frames (próximas stories)
   var framesCount = 0; // Mock
   
   // Criar manifest
   var manifestKey = prefixBuilder.BuildManifestKey(input.Output.ManifestPrefix, input.Chunk.ChunkId);
   var manifest = new
   {
       chunkId = input.Chunk.ChunkId,
       videoId = input.VideoId,
       status = "completed",
       framesCount = framesCount,
       startSec = input.Chunk.StartSec,
       endSec = input.Chunk.EndSec,
       processedAt = DateTime.UtcNow.ToString("o")
   };
   
   await s3Service.PutJsonAsync(bucket, manifestKey, manifest, ct);
   logger.LogInformation("Manifest written to s3://{Bucket}/{Key}", bucket, manifestKey);
   
   // Escrever done marker
   var doneKey = prefixBuilder.BuildDoneMarkerKey(input.Output.ManifestPrefix, input.Chunk.ChunkId);
   var doneMarker = new
   {
       completedAt = DateTime.UtcNow.ToString("o")
   };
   
   await s3Service.PutJsonAsync(bucket, doneKey, doneMarker, ct);
   logger.LogInformation("Done marker written to s3://{Bucket}/{Key}", bucket, doneKey);
   
   // Retornar output
   return new ChunkProcessorOutput(
       ChunkId: input.Chunk.ChunkId,
       Status: ProcessingStatus.SUCCEEDED,
       FramesCount: framesCount,
       Manifest: new ManifestInfo(bucket, manifestKey)
   );
   ```
2. Adicionar using: `using System;`

## Formas de Teste
1. **Teste de integração local:** rodar handler localmente com payload real, verificar que artefatos aparecem no S3
2. **Mock S3:** verificar que `PutJsonAsync` foi chamado 2 vezes (manifest + done) com chaves corretas
3. **Idempotência real:** executar handler duas vezes com mesmo payload, segunda vez deve retornar sucesso sem gravar novamente

## Critérios de Aceite da Subtask
- [ ] `ProcessChunkUseCase` escreve `manifest.json` com estrutura: chunkId, videoId, status, framesCount, startSec, endSec, processedAt
- [ ] `ProcessChunkUseCase` escreve `done.json` com: completedAt (timestamp ISO 8601)
- [ ] Ambos os artefatos gravados no S3 no prefixo determinístico correto
- [ ] Use case retorna `ChunkProcessorOutput` com status SUCCEEDED e manifest apontando para chave correta
- [ ] Logs mostram mensagens: "Manifest written" e "Done marker written"
- [ ] Teste de integração local confirma que artefatos são criados no S3
- [ ] Segunda execução com mesmo payload não grava artefatos novamente (idempotência)
