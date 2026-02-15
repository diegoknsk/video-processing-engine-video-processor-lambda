# Storie-07: Upload S3 Frames + Manifest + Log Mensageria XPTO

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do Lambda Worker, quero fazer upload dos frames extraídos e manifest.json para bucket S3 com prefixos determinísticos, e logar mensagem simulando envio para mensageria, para completar o fluxo básico de processamento de vídeo.

## Objetivo
Criar port `IS3Service` e implementação para operações S3, definir convenção de prefixos `processed/{videoId}/{chunkId}/`, fazer upload de todos os frames para S3, gerar manifest.json com metadados (videoId, chunkId, framesCount, frameKeys), fazer upload do manifest, logar "Processamento concluído. Enviado para mensageria XPTO" (mock, sem integração real), e validar fluxo end-to-end.

## Escopo Técnico
- **Tecnologias:** AWSSDK.S3, C# 13, async I/O, System.Text.Json
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Domain/Ports/IS3Service.cs` (port para S3)
  - `src/VideoProcessor.Infra.S3/Services/S3Service.cs` (implementação)
  - `src/VideoProcessor.Infra.S3/VideoProcessor.Infra.S3.csproj` (novo projeto)
  - `src/VideoProcessor.Application/Services/S3PrefixBuilder.cs` (convenção de prefixos)
  - `src/VideoProcessor.Application/Models/ManifestModel.cs` (estrutura do manifest)
  - `src/VideoProcessor.Application/UseCases/ProcessVideoUseCase.cs` (adicionar upload S3)
  - `src/VideoProcessor.Lambda/Function.cs` (configurar DI para S3Service)
  - `tests/VideoProcessor.Tests.Unit/Application/Services/S3PrefixBuilderTests.cs`
  - `tests/VideoProcessor.Tests.Unit/Infra/Services/S3ServiceTests.cs`
  - `docs/S3_CONVENTIONS.md` (documentar estrutura de prefixos)
- **Componentes:** S3Service, S3PrefixBuilder, ManifestModel, integração S3
- **Pacotes/Dependências:**
  - AWSSDK.S3 (3.7.400 ou superior) — já incluído na Storie-01

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-06 concluída (processamento rodando no Lambda)
  - Bucket S3 criado e Lambda com permissões (PutObject)
- **Riscos:**
  - Upload de muitos frames pode exceder timeout Lambda (mitigar: upload paralelo com SemaphoreSlim)
  - Falha parcial de upload pode deixar estado inconsistente (mitigar: não implementar idempotência ainda, aceitar)
  - Nome de bucket incorreto ou permissões faltando (mitigar: validar configuração, documentar troubleshooting)
- **Pré-condições:**
  - Bucket S3 criado (ex: `video-processing-frames-dev`)
  - Lambda com IAM role permitindo `s3:PutObject` no bucket

## Subtasks
- [Subtask 01: Criar port IS3Service e implementação S3Service](./subtask/Subtask-01-Criar_Port_S3Service.md)
- [Subtask 02: Criar S3PrefixBuilder para convenção determinística](./subtask/Subtask-02-Criar_S3PrefixBuilder.md)
- [Subtask 03: Implementar upload de frames para S3](./subtask/Subtask-03-Implementar_Upload_Frames_S3.md)
- [Subtask 04: Gerar e fazer upload de manifest.json](./subtask/Subtask-04-Gerar_Upload_Manifest.md)
- [Subtask 05: Integrar S3 no use case e validar fluxo end-to-end](./subtask/Subtask-05-Integrar_S3_Validar_E2E.md)

## Critérios de Aceite da História
- [ ] Port `IS3Service` criado com métodos: `UploadFileAsync(bucket, key, filePath)`, `UploadJsonAsync<T>(bucket, key, obj)`
- [ ] Implementação `S3Service` usando AWSSDK.S3 (AmazonS3Client)
- [ ] Novo projeto `VideoProcessor.Infra.S3` criado para isolar dependências S3
- [ ] `S3PrefixBuilder` gera prefixo determinístico: `processed/{videoId}/{chunkId}/`
- [ ] Upload de todos os frames para S3: `processed/{videoId}/{chunkId}/frame_0001_0s.jpg`, etc.
- [ ] `ManifestModel` criado com campos: videoId, chunkId, framesCount, frameKeys (array de S3 keys)
- [ ] Manifest.json gerado e uploaded: `processed/{videoId}/{chunkId}/manifest.json`
- [ ] Estrutura do manifest: `{ "videoId": "...", "chunkId": "...", "framesCount": 17, "frameKeys": [...], "completedAt": "..." }`
- [ ] Log final: "Processamento concluído. Enviado para mensageria XPTO" (mock, sem integração real)
- [ ] Lambda executa fluxo completo: processar → gerar frames → upload S3 → retornar sucesso
- [ ] Frames visíveis no bucket S3 via Console AWS
- [ ] Testes unitários cobrem: S3Service (mock S3), S3PrefixBuilder (prefixos corretos), upload de frames
- [ ] `docs/S3_CONVENTIONS.md` documenta estrutura de chaves e organização

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
