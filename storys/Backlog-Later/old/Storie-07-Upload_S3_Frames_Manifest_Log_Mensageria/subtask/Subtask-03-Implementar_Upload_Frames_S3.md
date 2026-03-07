# Subtask 03: Implementar upload de frames para S3

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Implementar lógica de upload de frames gerados em /tmp para bucket S3, usando prefixos determinísticos e processamento paralelo para performance.

## Tarefas
1. Atualizar `ProcessVideoUseCase` para receber `IS3Service`, videoId, chunkId, bucketName
2. Após extração de frames, fazer upload de cada frame:
   ```csharp
   foreach (var framePath in result.FramePaths)
   {
       var frameName = Path.GetFileName(framePath);
       var s3Key = S3PrefixBuilder.BuildFrameKey(videoId, chunkId, frameName);
       await s3Service.UploadFileAsync(bucketName, s3Key, framePath);
   }
   ```
3. Otimizar com upload paralelo usando `SemaphoreSlim` (max 5 simultâneos):
   ```csharp
   var semaphore = new SemaphoreSlim(5);
   var tasks = result.FramePaths.Select(async framePath => { ... });
   await Task.WhenAll(tasks);
   ```
4. Adicionar log de progresso: "Uploading frames: 10/17 concluídos"
5. Retornar lista de S3 keys dos frames uploaded

## Critérios de Aceite
- [ ] `ProcessVideoUseCase` recebe `IS3Service`, videoId, chunkId, bucketName
- [ ] Todos os frames locais são uploaded para S3
- [ ] Prefixos S3 corretos: `processed/{videoId}/{chunkId}/frame_0001_0s.jpg`
- [ ] Upload paralelo implementado (max 5 simultâneos)
- [ ] Log mostra progresso de upload
- [ ] Retorna lista de S3 keys dos frames
- [ ] Frames visíveis no bucket S3 via Console AWS

## Notas Técnicas
- `SemaphoreSlim` controla concorrência para não sobrecarregar rede/Lambda
- Capturar exceções de S3 e logar detalhes
- Upload pode levar tempo: considerar timeout Lambda (300s deve ser suficiente para ~20 frames)
- Frames pequenos (JPEG qualidade 85) devem ter ~100-500KB cada
