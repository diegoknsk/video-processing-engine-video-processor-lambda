# Subtask 04: Gerar e fazer upload de manifest.json

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Criar model `ManifestModel`, gerar manifest.json contendo metadados do processamento (videoId, chunkId, framesCount, frameKeys, timestamp), e fazer upload para S3.

## Tarefas
1. Criar `src/VideoProcessor.Application/Models/ManifestModel.cs`:
   ```csharp
   public record ManifestModel(
       string VideoId,
       string ChunkId,
       int FramesCount,
       List<string> FrameKeys,
       DateTime CompletedAt);
   ```
2. No `ProcessVideoUseCase`, após upload de frames:
   - Criar instância de `ManifestModel`
   - Preencher com dados: videoId, chunkId, framesCount, frameKeys (S3 keys), DateTime.UtcNow
   - Fazer upload usando `s3Service.UploadJsonAsync()`
3. Key do manifest: `S3PrefixBuilder.BuildManifestKey(videoId, chunkId)`
4. Adicionar log: "Manifest uploaded: processed/{videoId}/{chunkId}/manifest.json"

## Critérios de Aceite
- [ ] Model `ManifestModel` criado com campos: VideoId, ChunkId, FramesCount, FrameKeys, CompletedAt
- [ ] Manifest gerado após upload de frames
- [ ] Manifest contém todos os campos preenchidos corretamente
- [ ] Estrutura JSON:
   ```json
   {
     "videoId": "video-123",
     "chunkId": "chunk-001",
     "framesCount": 17,
     "frameKeys": [
       "processed/video-123/chunk-001/frame_0001_0s.jpg",
       "processed/video-123/chunk-001/frame_0002_20s.jpg",
       ...
     ],
     "completedAt": "2026-02-15T17:00:00Z"
   }
   ```
- [ ] Manifest uploaded para S3
- [ ] Manifest visível no Console S3 e pode ser baixado
- [ ] Log mostra key do manifest

## Notas Técnicas
- Usar `System.Text.Json.JsonSerializer.Serialize()` com opções: `WriteIndented = true`
- CompletedAt em formato ISO 8601: `DateTime.UtcNow.ToString("o")`
- FrameKeys deve conter apenas as keys S3 (não paths locais)
