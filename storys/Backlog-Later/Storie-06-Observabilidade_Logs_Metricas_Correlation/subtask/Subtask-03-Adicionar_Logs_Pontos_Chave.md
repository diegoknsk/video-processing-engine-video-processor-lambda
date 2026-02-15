# Subtask 03: Adicionar Logs em Pontos-Chave do Fluxo

## Descrição
Identificar pontos-chave do fluxo de processamento (início, validação, idempotência, S3 operations, sucesso, falha) e adicionar logs estruturados com informações relevantes em cada ponto.

## Passos de Implementação
1. No `Function.cs`, adicionar logs:
   - **Início do handler:**
     ```csharp
     logger.LogInformationWithContext($"Handler invoked. RequestId={context.AwsRequestId}");
     ```
   - **Após deserialização:**
     ```csharp
     logger.LogInformationWithContext($"Input deserialized. ContractVersion={input.ContractVersion}");
     ```
   - **Após validação (se sucesso):**
     ```csharp
     logger.LogInformationWithContext("Input validation passed");
     ```
   - **Antes de chamar use case:**
     ```csharp
     logger.LogInformationWithContext("Invoking ProcessChunkUseCase");
     ```
   - **Após sucesso:**
     ```csharp
     logger.LogInformationWithContext($"Chunk processed successfully. Status={output.Status}");
     ```
   - **Em caso de erro:**
     ```csharp
     logger.LogErrorWithContext(ex, $"Error processing chunk. ErrorType={errorInfo.Type}, Retryable={errorInfo.Retryable}");
     ```
2. No `ProcessChunkUseCase`, adicionar logs:
   - **Verificação de idempotência (done exists):**
     ```csharp
     logger.LogInformationWithContext("Chunk already processed (done marker exists). Returning cached result.");
     ```
   - **Início do processamento:**
     ```csharp
     logger.LogInformationWithContext($"Processing new chunk. StartSec={input.Chunk.StartSec}, EndSec={input.Chunk.EndSec}");
     ```
   - **Após escrever manifest:**
     ```csharp
     logger.LogInformationWithContext($"Manifest written to S3. Key={manifestKey}");
     ```
   - **Após escrever done marker:**
     ```csharp
     logger.LogInformationWithContext($"Done marker written to S3. Key={doneKey}");
     ```
3. Adicionar timing logs (opcional):
   ```csharp
   var stopwatch = Stopwatch.StartNew();
   // ... processamento ...
   stopwatch.Stop();
   logger.LogInformationWithContext($"Chunk processing completed in {stopwatch.ElapsedMilliseconds}ms");
   ```

## Formas de Teste
1. **Execução local:** rodar handler com payload de teste, verificar que todos os logs aparecem na ordem esperada
2. **CloudWatch:** após deploy, executar Step Functions e verificar logs no CloudWatch Logs
3. **Logs Insights:** criar query para filtrar logs por videoId e verificar fluxo completo

## Critérios de Aceite da Subtask
- [ ] Logs adicionados em: início do handler, após deserialização, após validação, antes/depois do use case, sucesso, falha
- [ ] Logs adicionados no use case em: verificação de idempotência, início do processamento, escrita de manifest, escrita de done marker
- [ ] Logs incluem informações relevantes: RequestId, ContractVersion, Status, ErrorType, S3 keys, duração
- [ ] Todos os logs usam extension methods com correlation context
- [ ] Execução local mostra fluxo completo nos logs (mínimo 8 mensagens de log)
- [ ] Logs estruturados permitem queries no CloudWatch Logs Insights por videoId/chunkId
