# Subtask 04: Atualizar Function Handler para Usar Models Tipados

## Descrição
Refatorar `Function.cs` para deserializar `JsonDocument` em `ChunkProcessorInput`, processar (mock), serializar `ChunkProcessorOutput` de volta para `JsonDocument`, e tratar exceções de contrato no handler.

## Passos de Implementação
1. Atualizar assinatura do `FunctionHandler` (mantém `JsonDocument` por compatibilidade com Lambda):
   ```csharp
   public async Task<JsonDocument> FunctionHandler(JsonDocument inputDoc, ILambdaContext context)
   ```
2. No início do handler, deserializar:
   ```csharp
   var inputJson = inputDoc.RootElement.GetRawText();
   var input = JsonSerializer.Deserialize<ChunkProcessorInput>(inputJson, _jsonOptions);
   
   if (input == null)
       throw new InvalidOperationException("Failed to deserialize input");
   ```
3. Validar `contractVersion`:
   ```csharp
   _versionValidator.Validate(input.ContractVersion);
   ```
4. Processar chunk (mock por enquanto):
   ```csharp
   context.Logger.LogInformation($"Processing videoId={input.VideoId}, chunkId={input.Chunk.ChunkId}");
   
   // Mock: criar output SUCCEEDED
   var output = new ChunkProcessorOutput(
       ChunkId: input.Chunk.ChunkId,
       Status: ProcessingStatus.SUCCEEDED,
       FramesCount: 0,
       Manifest: new ManifestInfo(
           Bucket: input.Output.ManifestBucket,
           Key: $"{input.Output.ManifestPrefix}/{input.Chunk.ChunkId}/manifest.json"
       )
   );
   ```
5. Serializar output de volta para `JsonDocument`:
   ```csharp
   var outputJson = JsonSerializer.Serialize(output, _jsonOptions);
   return JsonDocument.Parse(outputJson);
   ```
6. Adicionar try-catch para `UnsupportedContractVersionException`:
   ```csharp
   catch (UnsupportedContractVersionException ex)
   {
       context.Logger.LogError($"Unsupported contract version: {ex.Message}");
       var errorOutput = new ChunkProcessorOutput(
           ChunkId: input?.Chunk?.ChunkId ?? "unknown",
           Status: ProcessingStatus.FAILED,
           FramesCount: 0,
           Error: new ErrorInfo("ValidationError", ex.Message, Retryable: false)
       );
       return JsonDocument.Parse(JsonSerializer.Serialize(errorOutput));
   }
   ```
7. Configurar `JsonSerializerOptions` no construtor:
   ```csharp
   private readonly JsonSerializerOptions _jsonOptions = new()
   {
       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
       DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
   };
   ```

## Formas de Teste
1. **Teste local:** criar console app que invoca handler com payload JSON real
2. **Smoke test:** handler retorna output estruturado (não mais mock genérico)
3. **Teste de erro:** payload com `contractVersion: "999"` retorna FAILED com error.retryable = false

## Critérios de Aceite da Subtask
- [ ] Handler deserializa input para `ChunkProcessorInput` tipado
- [ ] Handler serializa output de `ChunkProcessorOutput` para `JsonDocument`
- [ ] Validação de `contractVersion` executada antes do processamento
- [ ] Output mockado contém todos os campos esperados (chunkId, status, framesCount, manifest)
- [ ] Tratamento de `UnsupportedContractVersionException` retorna FAILED com error.retryable = false
- [ ] JsonSerializerOptions configurado com camelCase e ignora nulls
- [ ] Handler compila e executa localmente com payload de teste
