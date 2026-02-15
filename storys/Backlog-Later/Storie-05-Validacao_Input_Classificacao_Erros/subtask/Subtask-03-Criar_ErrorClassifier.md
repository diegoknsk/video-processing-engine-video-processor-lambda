# Subtask 03: Criar ErrorClassifier para Classificar Exceções

## Descrição
Criar serviço `ErrorClassifier` que analisa tipos de exceção e determina se erro é retryable (S3, rede) ou não-retryable (validação, lógica), retornando `ErrorInfo` estruturado.

## Passos de Implementação
1. Criar `src/VideoProcessor.Application/Services/ErrorClassifier.cs`:
   ```csharp
   using Amazon.S3;
   
   public interface IErrorClassifier
   {
       ErrorInfo Classify(Exception exception, string chunkId);
   }
   
   public class ErrorClassifier : IErrorClassifier
   {
       public ErrorInfo Classify(Exception exception, string chunkId)
       {
           return exception switch
           {
               ChunkValidationException validationEx => new ErrorInfo(
                   Type: "ValidationError",
                   Message: validationEx.Message,
                   Retryable: false
               ),
               
               UnsupportedContractVersionException versionEx => new ErrorInfo(
                   Type: "UnsupportedContractVersion",
                   Message: versionEx.Message,
                   Retryable: false
               ),
               
               AmazonS3Exception s3Ex => new ErrorInfo(
                   Type: "S3Error",
                   Message: $"S3 operation failed: {s3Ex.Message}",
                   Retryable: IsRetryableS3Exception(s3Ex)
               ),
               
               HttpRequestException httpEx => new ErrorInfo(
                   Type: "NetworkError",
                   Message: $"Network error: {httpEx.Message}",
                   Retryable: true
               ),
               
               _ => new ErrorInfo(
                   Type: "UnexpectedError",
                   Message: exception.Message,
                   Retryable: false
               )
           };
       }
       
       private static bool IsRetryableS3Exception(AmazonS3Exception ex)
       {
           // Erros retryable: 500, 503, throttling
           return ex.StatusCode == System.Net.HttpStatusCode.InternalServerError
               || ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
               || ex.ErrorCode == "RequestTimeout"
               || ex.ErrorCode == "SlowDown";
       }
   }
   ```
2. Registrar no DI:
   ```csharp
   services.AddSingleton<IErrorClassifier, ErrorClassifier>();
   ```

## Formas de Teste
1. **Teste unitário:** classificar `ChunkValidationException` → retryable = false
2. **Teste unitário:** classificar `AmazonS3Exception` (503) → retryable = true
3. **Teste unitário:** classificar `AmazonS3Exception` (404) → retryable = false
4. **Teste unitário:** classificar `HttpRequestException` → retryable = true

## Critérios de Aceite da Subtask
- [ ] `IErrorClassifier` e `ErrorClassifier` criados
- [ ] Classificação correta: `ChunkValidationException` e `UnsupportedContractVersionException` → não-retryable
- [ ] Classificação correta: `AmazonS3Exception` (500, 503, throttling) → retryable
- [ ] Classificação correta: `AmazonS3Exception` (404, 403) → não-retryable
- [ ] Classificação correta: `HttpRequestException` → retryable
- [ ] Exceções desconhecidas → não-retryable (conservador)
- [ ] Testes unitários cobrem todos os tipos de exceção
- [ ] Classifier registrado no DI
