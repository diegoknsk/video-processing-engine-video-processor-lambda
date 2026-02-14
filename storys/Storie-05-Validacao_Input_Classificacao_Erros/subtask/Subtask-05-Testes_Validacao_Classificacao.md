# Subtask 05: Criar Testes Unitários de Validação e Classificação

## Descrição
Criar testes unitários que validam regras do `ChunkProcessorInputValidator`, comportamento da `ChunkValidationException`, e classificação de exceções pelo `ErrorClassifier`.

## Passos de Implementação
1. Criar `tests/VideoProcessor.Tests.Unit/Application/Validators/ChunkProcessorInputValidatorTests.cs`:
   - Teste: input válido completo → `IsValid = true`
   - Teste: contractVersion vazio → falha com mensagem "ContractVersion is required"
   - Teste: videoId vazio → falha
   - Teste: chunkId vazio → falha
   - Teste: startSec < 0 → falha
   - Teste: endSec <= startSec → falha com mensagem "EndSec must be > StartSec"
   - Teste: source.bucket vazio → falha
   - Teste: output.manifestBucket vazio → falha
   - Teste: múltiplos erros → retorna todos os erros de validação
2. Criar `tests/VideoProcessor.Tests.Unit/Application/Services/ErrorClassifierTests.cs`:
   - Teste: `ChunkValidationException` → ErrorInfo com retryable = false, type = "ValidationError"
   - Teste: `UnsupportedContractVersionException` → retryable = false
   - Teste: `AmazonS3Exception` (StatusCode 503) → retryable = true, type = "S3Error"
   - Teste: `AmazonS3Exception` (StatusCode 404) → retryable = false
   - Teste: `AmazonS3Exception` (ErrorCode "SlowDown") → retryable = true
   - Teste: `HttpRequestException` → retryable = true, type = "NetworkError"
   - Teste: exceção genérica (ex.: `InvalidOperationException`) → retryable = false, type = "UnexpectedError"
3. Criar `tests/VideoProcessor.Tests.Unit/Domain/Exceptions/ChunkValidationExceptionTests.cs`:
   - Teste: criar exceção com lista de `ValidationFailure` → mensagem concatena todos os erros
   - Teste: propriedade `ValidationErrors` contém lista correta
4. Usar FluentAssertions:
   ```csharp
   var result = validator.Validate(input);
   result.IsValid.Should().BeFalse();
   result.Errors.Should().ContainSingle(e => e.ErrorMessage == "VideoId is required");
   
   var errorInfo = classifier.Classify(exception, "chunk-0");
   errorInfo.Retryable.Should().BeTrue();
   errorInfo.Type.Should().Be("S3Error");
   ```

## Formas de Teste
1. **Execução:** `dotnet test tests/VideoProcessor.Tests.Unit --filter FullyQualifiedName~Validator`
2. **Cobertura:** validator e classifier devem ter cobertura 100%
3. **Casos de borda:** testar limites (startSec = 0, endSec = startSec + 0.001)

## Critérios de Aceite da Subtask
- [ ] Testes de `ChunkProcessorInputValidator` cobrem: input válido, cada campo obrigatório faltando, ranges inválidos, múltiplos erros
- [ ] Testes de `ErrorClassifier` cobrem: todos os tipos de exceção (validação, S3, rede, genérica), cenários retryable e não-retryable
- [ ] Testes de `ChunkValidationException` cobrem: criação com múltiplos erros, mensagem concatenada
- [ ] Todos os testes passam: `dotnet test tests/VideoProcessor.Tests.Unit`
- [ ] Cobertura: validator ≥ 100%, classifier ≥ 100%
- [ ] FluentAssertions usado para asserções legíveis
