# Subtask 05: Integrar S3 no use case e validar fluxo end-to-end

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Integrar completamente `IS3Service` no use case e handler Lambda, adicionar log mockado de mensageria, validar fluxo end-to-end completo, e documentar convenções S3.

## Tarefas
1. Atualizar `Function.cs`:
   - Configurar DI: registrar `IS3Service` → `S3Service` com `AmazonS3Client`
   - Atualizar input para incluir: videoId, chunkId, bucketName, intervalSeconds
   - Passar dependências para `ProcessVideoUseCase`
2. No `ProcessVideoUseCase`, após upload de manifest:
   - Logar: `"Processamento concluído. Enviado para mensageria XPTO"`
   - Retornar status SUCCEEDED com metadados completos
3. Testar fluxo end-to-end na AWS:
   - Invocar Lambda com input:
     ```json
     {
       "videoId": "video-123",
       "chunkId": "chunk-001",
       "bucketName": "video-processing-frames-dev",
       "intervalSeconds": 20,
       "videoBase64": "..."
     }
     ```
   - Validar resposta
   - Verificar frames no S3
   - Verificar manifest.json no S3
   - Validar logs CloudWatch
4. Criar `docs/S3_CONVENTIONS.md` documentando estrutura de chaves

## Critérios de Aceite
- [ ] DI configurado para injetar `S3Service` no use case
- [ ] Input Lambda inclui: videoId, chunkId, bucketName, intervalSeconds
- [ ] Lambda executa fluxo completo: processar → upload frames → upload manifest → log mensageria
- [ ] Log final: "Processamento concluído. Enviado para mensageria XPTO"
- [ ] Resposta retorna: `{ "status": "SUCCEEDED", "framesCount": 17, "manifestKey": "...", "message": "Enviado para mensageria XPTO" }`
- [ ] Frames visíveis no bucket S3 com prefixos corretos
- [ ] Manifest.json no S3 com estrutura correta
- [ ] Logs CloudWatch mostram fluxo completo
- [ ] `docs/S3_CONVENTIONS.md` documenta:
  - Estrutura de prefixos
  - Exemplos de keys
  - Como listar frames de um chunk
  - Como baixar manifest

## Notas Técnicas
- DI: `services.AddSingleton<IAmazonS3, AmazonS3Client>()`
- DI: `services.AddScoped<IS3Service, S3Service>()`
- Bucket name pode vir de variável de ambiente: `Environment.GetEnvironmentVariable("BUCKET_NAME")`
- Mensageria será implementada futuramente (por ora apenas log)
- Documentar em README o fluxo completo implementado
