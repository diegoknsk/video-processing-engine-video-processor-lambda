# Subtask 03: Implementar ProcessChunkUseCase com Verificação de Idempotência

## Descrição
Criar use case `ProcessChunkUseCase` que recebe `ChunkProcessorInput`, verifica se `done.json` existe no S3 (idempotência), e retorna output mockado SUCCEEDED sem reprocessar se já existir.

## Passos de Implementação
1. Criar `src/VideoProcessor.Application/UseCases/ProcessChunkUseCase.cs`:
   ```csharp
   public interface IProcessChunkUseCase
   {
       Task<ChunkProcessorOutput> ExecuteAsync(ChunkProcessorInput input, CancellationToken ct = default);
   }
   
   public class ProcessChunkUseCase(
       IS3Service s3Service,
       IPrefixBuilder prefixBuilder,
       ILogger<ProcessChunkUseCase> logger
   ) : IProcessChunkUseCase
   {
       public async Task<ChunkProcessorOutput> ExecuteAsync(ChunkProcessorInput input, CancellationToken ct)
       {
           var doneKey = prefixBuilder.BuildDoneMarkerKey(input.Output.ManifestPrefix, input.Chunk.ChunkId);
           var bucket = input.Output.ManifestBucket;
           
           // Verificar idempotência
           if (await s3Service.ExistsAsync(bucket, doneKey, ct))
           {
               logger.LogInformation("Chunk {ChunkId} already processed (done marker exists). Returning cached result.", input.Chunk.ChunkId);
               
               var manifestKey = prefixBuilder.BuildManifestKey(input.Output.ManifestPrefix, input.Chunk.ChunkId);
               return new ChunkProcessorOutput(
                   ChunkId: input.Chunk.ChunkId,
                   Status: ProcessingStatus.SUCCEEDED,
                   FramesCount: 0, // Mock
                   Manifest: new ManifestInfo(bucket, manifestKey)
               );
           }
           
           logger.LogInformation("Processing new chunk {ChunkId}...", input.Chunk.ChunkId);
           
           // Processar chunk (mock por enquanto)
           // TODO: extrair frames em próximas stories
           
           // Por enquanto, retornar null (próxima subtask gravará artefatos)
           return null!; // Subtask 04 completará isso
       }
   }
   ```
2. Registrar no DI:
   ```csharp
   services.AddScoped<IProcessChunkUseCase, ProcessChunkUseCase>();
   ```
3. No `Function.cs`, chamar use case após validar input:
   ```csharp
   var input = JsonSerializer.Deserialize<ChunkProcessorInput>(inputJson);
   _versionValidator.Validate(input.ContractVersion);
   
   var useCase = _serviceProvider.GetRequiredService<IProcessChunkUseCase>();
   var output = await useCase.ExecuteAsync(input, context.RemainingTime);
   
   return JsonDocument.Parse(JsonSerializer.Serialize(output));
   ```

## Formas de Teste
1. **Mock S3:** mock `ExistsAsync` retorna true → use case retorna SUCCEEDED sem processar
2. **Mock S3:** mock `ExistsAsync` retorna false → use case prossegue para processamento
3. **Teste de logs:** verificar que mensagem "already processed" é logada quando idempotente

## Critérios de Aceite da Subtask
- [ ] `IProcessChunkUseCase` e `ProcessChunkUseCase` criados
- [ ] Use case verifica existência de `done.json` antes de processar
- [ ] Se `done.json` existir: retorna SUCCEEDED com manifest apontando para chave correta, não reprocessa
- [ ] Se não existir: loga "Processing new chunk" e prossegue (mock por enquanto)
- [ ] Use case registrado no DI como Scoped
- [ ] Handler integrado com use case (chama `ExecuteAsync` e retorna output serializado)
- [ ] Testes unitários (com mock de S3Service) cobrem cenário idempotente e não-idempotente
