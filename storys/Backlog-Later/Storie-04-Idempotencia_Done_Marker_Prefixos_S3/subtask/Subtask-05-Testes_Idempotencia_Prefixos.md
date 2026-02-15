# Subtask 05: Criar Testes Unitários de Idempotência e Prefixos

## Descrição
Criar testes unitários que validam lógica de idempotência (verificação de done marker), construção de prefixos/chaves, e comportamento do use case em cenários de chunk já processado vs chunk novo.

## Passos de Implementação
1. Criar `tests/VideoProcessor.Tests.Unit/Application/Services/PrefixBuilderTests.cs`:
   - Teste: `BuildManifestKey` gera chave correta
   - Teste: `BuildDoneMarkerKey` gera chave correta
   - Teste: `BuildChunkPrefix` normaliza trailing slash (com e sem slash no input)
   - Teste: prefixos vazios ou edge cases (se aplicável)
2. Criar `tests/VideoProcessor.Tests.Unit/Application/UseCases/ProcessChunkUseCaseTests.cs`:
   - Setup: mock de `IS3Service`, `IPrefixBuilder`, `ILogger`
   - Teste: **Chunk já processado (done exists)**
     - Mock `ExistsAsync` retorna true
     - Executar use case
     - Verificar: output SUCCEEDED, `PutJsonAsync` NÃO foi chamado, log "already processed"
   - Teste: **Chunk novo (done não exists)**
     - Mock `ExistsAsync` retorna false
     - Executar use case
     - Verificar: output SUCCEEDED, `PutJsonAsync` chamado 2 vezes (manifest + done), log "Processing new chunk"
   - Teste: **Manifest contém campos esperados**
     - Capturar argumentos de `PutJsonAsync` para manifestKey
     - Verificar JSON contém: chunkId, videoId, status="completed", framesCount=0, processedAt
   - Teste: **Done marker contém timestamp**
     - Capturar argumentos de `PutJsonAsync` para doneKey
     - Verificar JSON contém: completedAt
3. Usar Moq para mockar `IS3Service`:
   ```csharp
   var s3Mock = new Mock<IS3Service>();
   s3Mock.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(false);
   
   s3Mock.Verify(s => s.PutJsonAsync(
       It.IsAny<string>(), 
       It.Is<string>(k => k.EndsWith("manifest.json")), 
       It.IsAny<object>(), 
       It.IsAny<CancellationToken>()), 
       Times.Once);
   ```
4. Usar FluentAssertions para asserções legíveis

## Formas de Teste
1. **Execução:** `dotnet test tests/VideoProcessor.Tests.Unit --filter FullyQualifiedName~ProcessChunkUseCase`
2. **Cobertura:** use case deve ter cobertura ≥ 90%
3. **Validação de mocks:** verificar que métodos esperados foram chamados (ou não) corretamente

## Critérios de Aceite da Subtask
- [ ] Testes de `PrefixBuilder` cobrem: manifest key, done key, normalização de trailing slash
- [ ] Testes de `ProcessChunkUseCase` cobrem: chunk já processado (idempotente), chunk novo (processa e grava)
- [ ] Teste verifica que `PutJsonAsync` NÃO é chamado quando done marker existe
- [ ] Teste verifica que `PutJsonAsync` é chamado 2 vezes (manifest + done) quando chunk é novo
- [ ] Teste valida estrutura do manifest JSON (campos obrigatórios presentes)
- [ ] Teste valida estrutura do done marker JSON (completedAt presente)
- [ ] Todos os testes passam: `dotnet test tests/VideoProcessor.Tests.Unit`
- [ ] Cobertura do use case ≥ 90%
