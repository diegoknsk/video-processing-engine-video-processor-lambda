# Subtask 04: Adicionar Tratamento de Exceções no Handler

## Descrição
Envolver chamada do use case em try-catch no handler, classificar exceções com `ErrorClassifier`, retornar FAILED com error para exceções não-retryable, e re-lançar exceções retryable para Step Functions aplicar retry.

## Passos de Implementação
1. No `Function.cs`, refatorar `FunctionHandler`:
   ```csharp
   public async Task<JsonDocument> FunctionHandler(JsonDocument inputDoc, ILambdaContext context)
   {
       ChunkProcessorInput? input = null;
       try
       {
           var inputJson = inputDoc.RootElement.GetRawText();
           input = JsonSerializer.Deserialize<ChunkProcessorInput>(inputJson, _jsonOptions);
           
           if (input == null)
               throw new InvalidOperationException("Failed to deserialize input");
           
           var useCase = _serviceProvider.GetRequiredService<IProcessChunkUseCase>();
           var output = await useCase.ExecuteAsync(input, CancellationToken.None);
           
           return JsonDocument.Parse(JsonSerializer.Serialize(output, _jsonOptions));
       }
       catch (Exception ex)
       {
           context.Logger.LogError($"Error processing chunk: {ex}");
           
           var classifier = _serviceProvider.GetRequiredService<IErrorClassifier>();
           var errorInfo = classifier.Classify(ex, input?.Chunk?.ChunkId ?? "unknown");
           
           if (errorInfo.Retryable)
           {
               // Re-lançar para Step Functions aplicar retry
               context.Logger.LogWarning($"Retryable error detected. Throwing exception for retry.");
               throw;
           }
           
           // Erro não-retryable: retornar FAILED
           var errorOutput = new ChunkProcessorOutput(
               ChunkId: input?.Chunk?.ChunkId ?? "unknown",
               Status: ProcessingStatus.FAILED,
               FramesCount: 0,
               Error: errorInfo
           );
           
           return JsonDocument.Parse(JsonSerializer.Serialize(errorOutput, _jsonOptions));
       }
   }
   ```
2. Adicionar logs estruturados com severidade correta (LogError para erros, LogWarning para retryable)

## Formas de Teste
1. **Teste local:** input inválido retorna FAILED com error.retryable = false
2. **Mock S3:** simular `AmazonS3Exception` (503) → exceção é re-lançada
3. **Mock validator:** simular `ChunkValidationException` → retorna FAILED sem re-lançar

## Critérios de Aceite da Subtask
- [ ] Handler envolve chamada do use case em try-catch
- [ ] Exceções são classificadas com `ErrorClassifier`
- [ ] Exceções retryable são re-lançadas (throw;) após log
- [ ] Exceções não-retryable retornam `ChunkProcessorOutput` com status FAILED e error detalhado
- [ ] Logs incluem: tipo de erro, se é retryable, mensagem
- [ ] Handler executa localmente com input inválido e retorna FAILED com error correto
- [ ] Handler executa localmente com erro S3 mockado (503) e lança exceção
