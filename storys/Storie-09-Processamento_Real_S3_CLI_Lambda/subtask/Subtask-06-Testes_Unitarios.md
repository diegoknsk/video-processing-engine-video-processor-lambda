# Subtask 06: Testes unitários — ProcessChunkUseCase e S3VideoStorage

## Descrição
Escrever testes unitários para os dois componentes novos da story: `ProcessChunkUseCase` (camada Application) e `S3VideoStorage` (camada Infra). Os testes devem usar mocks para isolar os ports, cobrir cenários de sucesso e falha e garantir que o comportamento esperado pela Lambda e pelo CLI modo AWS está correto. Meta de cobertura: ≥ 80% nos arquivos novos.

## Passos de implementação

### Projeto de testes
Os testes devem ser adicionados ao projeto existente `tests/VideoProcessor.Tests.Unit`. Criar as pastas:
- `tests/VideoProcessor.Tests.Unit/Application/UseCases/` → `ProcessChunkUseCaseTests.cs`
- `tests/VideoProcessor.Tests.Unit/Infra/S3/` → `S3VideoStorageTests.cs`

Verificar que `Moq` já está referenciado; adicionar se necessário:
```xml
<PackageReference Include="Moq" Version="4.*" />
```

### Testes de ProcessChunkUseCase

**Cenário 1 — Sucesso completo:**
- Mock de `IVideoFrameExtractor.ExtractFramesAsync` retornando `FrameExtractionResult` com 3 frames.
- Mock de `IS3VideoStorage.DownloadToTempAsync` retornando o path local.
- Mock de `IS3VideoStorage.UploadFramesAsync` retornando lista de 3 S3 keys.
- Invocar `ExecuteAsync` com input válido.
- Verificar: `output.Status == ProcessingStatus.Succeeded`, `output.FramesCount == 3`, `output.ChunkId == input.Chunk.ChunkId`.

**Cenário 2 — Falha no download (FileNotFoundException):**
- Mock de `DownloadToTempAsync` lançando `FileNotFoundException`.
- Verificar: `output.Status == ProcessingStatus.Failed`, `output.Error != null`, `output.FramesCount == 0`.

**Cenário 3 — Falha na extração (InvalidOperationException):**
- Mock de `DownloadToTempAsync` retornando path válido.
- Mock de `ExtractFramesAsync` lançando `InvalidOperationException("FFmpeg error")`.
- Verificar: `output.Status == ProcessingStatus.Failed`, `output.Error.Message` contém "FFmpeg error".

**Cenário 4 — Input inválido (null ou campos obrigatórios vazios):**
- Invocar com `input = null` ou `input.Source.Bucket = null`.
- Verificar que o use case retorna `Status = Failed` ou lança `ArgumentException` (definir comportamento esperado na Subtask 03 e testar consistentemente).

**Cenário 5 — Cleanup executado mesmo em falha:**
- Verificar (via verificação de calls no mock ou spy de `Directory.Delete`) que o cleanup de `/tmp` é tentado mesmo quando a extração falha.
  - *Nota: se necessário, extrair o cleanup para um método virtual ou injetável para permitir verificação em teste.*

### Testes de S3VideoStorage

**Cenário 1 — DownloadToTempAsync com sucesso:**
- Mock de `IAmazonS3.GetObjectAsync` retornando `GetObjectResponse` com stream válido.
- Verificar que o arquivo local foi gravado no path informado.
- Verificar que `GetObjectAsync` foi chamado com `Bucket` e `Key` corretos.

**Cenário 2 — DownloadToTempAsync com objeto não encontrado (404):**
- Mock de `IAmazonS3.GetObjectAsync` lançando `AmazonS3Exception` com `StatusCode = 404`.
- Verificar que `FileNotFoundException` é lançado pelo `S3VideoStorage`.

**Cenário 3 — UploadFramesAsync com lista de 3 frames:**
- Mock de `IAmazonS3.PutObjectAsync` retornando sucesso.
- Passar lista de 3 paths de frames.
- Verificar que `PutObjectAsync` foi chamado exatamente 3 vezes.
- Verificar que as S3 keys retornadas seguem o padrão `{prefix}{fileName}`.

**Cenário 4 — UploadFramesAsync normaliza prefixo sem barra final:**
- Passar `prefix = "processed/video-abc/chunk-001/frames"` (sem `/` final).
- Verificar que as keys geradas usam `"processed/video-abc/chunk-001/frames/"` (com `/`).

### Cobertura
Após implementar os testes, executar:
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory coverage
```
Verificar que `ProcessChunkUseCase` e `S3VideoStorage` têm cobertura ≥ 80%.

## Formas de teste
- `dotnet test` na raiz do repositório — todos os testes devem passar.
- `dotnet test --filter "FullyQualifiedName~ProcessChunkUseCase"` para rodar apenas os novos testes do use case.
- `dotnet test --filter "FullyQualifiedName~S3VideoStorage"` para rodar apenas os novos testes do storage.

## Critérios de aceite da subtask
- [ ] Arquivo `ProcessChunkUseCaseTests.cs` criado com ≥ 4 cenários testados (sucesso, falha download, falha extração, input inválido).
- [ ] Arquivo `S3VideoStorageTests.cs` criado com ≥ 4 cenários testados (sucesso download, 404 download, upload 3 frames, normalização de prefixo).
- [ ] `dotnet test` executa todos os testes (antigos e novos) sem falha.
- [ ] Cobertura dos novos arquivos ≥ 80% verificada via `dotnet test --collect:"XPlat Code Coverage"`.
- [ ] Testes são isolados (sem rede, sem filesystem real, sem AWS real) — apenas mocks.
