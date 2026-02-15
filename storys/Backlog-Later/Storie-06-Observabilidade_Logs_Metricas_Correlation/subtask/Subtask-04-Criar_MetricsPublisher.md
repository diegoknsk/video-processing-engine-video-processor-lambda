# Subtask 04: Criar MetricsPublisher e Publicar Métricas Customizadas

## Descrição
Criar port `IMetricsPublisher` e implementação `CloudWatchMetricsPublisher` para publicar métricas customizadas (duração, sucesso/falha, chunks processados) no CloudWatch Metrics usando Embedded Metric Format (EMF) ou PutMetricData.

## Passos de Implementação
1. Criar `src/VideoProcessor.Domain/Ports/IMetricsPublisher.cs`:
   ```csharp
   public interface IMetricsPublisher
   {
       Task PublishProcessingDurationAsync(string chunkId, long durationMs, CancellationToken ct = default);
       Task PublishProcessingResultAsync(string chunkId, ProcessingStatus status, CancellationToken ct = default);
       Task PublishChunkProcessedAsync(CancellationToken ct = default);
   }
   ```
2. Criar `src/VideoProcessor.Infra/Services/CloudWatchMetricsPublisher.cs`:
   - Usar Embedded Metric Format (EMF) para logs estruturados que viram métricas:
   ```csharp
   public class CloudWatchMetricsPublisher(ILogger<CloudWatchMetricsPublisher> logger) : IMetricsPublisher
   {
       private const string Namespace = "VideoProcessing/ChunkWorker";
       
       public Task PublishProcessingDurationAsync(string chunkId, long durationMs, CancellationToken ct)
       {
           // EMF: log estruturado com _aws metadata
           var metricLog = new
           {
               _aws = new
               {
                   Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                   CloudWatchMetrics = new[]
                   {
                       new
                       {
                           Namespace = Namespace,
                           Dimensions = new[] { new[] { "ChunkId" } },
                           Metrics = new[]
                           {
                               new { Name = "ProcessingDuration", Unit = "Milliseconds" }
                           }
                       }
                   }
               },
               ChunkId = chunkId,
               ProcessingDuration = durationMs
           };
           
           logger.LogInformation(JsonSerializer.Serialize(metricLog));
           return Task.CompletedTask;
       }
       
       public Task PublishProcessingResultAsync(string chunkId, ProcessingStatus status, CancellationToken ct)
       {
           var metricLog = new
           {
               _aws = new
               {
                   Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                   CloudWatchMetrics = new[]
                   {
                       new
                       {
                           Namespace = Namespace,
                           Dimensions = new[] { new[] { "Status" } },
                           Metrics = new[]
                           {
                               new { Name = "ProcessingResult", Unit = "Count" }
                           }
                       }
                   }
               },
               Status = status.ToString(),
               ProcessingResult = 1
           };
           
           logger.LogInformation(JsonSerializer.Serialize(metricLog));
           return Task.CompletedTask;
       }
       
       public Task PublishChunkProcessedAsync(CancellationToken ct)
       {
           var metricLog = new
           {
               _aws = new
               {
                   Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                   CloudWatchMetrics = new[]
                   {
                       new
                       {
                           Namespace = Namespace,
                           Dimensions = new[] { Array.Empty<string>() },
                           Metrics = new[]
                           {
                               new { Name = "ChunksProcessed", Unit = "Count" }
                           }
                       }
                   }
               },
               ChunksProcessed = 1
           };
           
           logger.LogInformation(JsonSerializer.Serialize(metricLog));
           return Task.CompletedTask;
       }
   }
   ```
3. Registrar no DI:
   ```csharp
   services.AddSingleton<IMetricsPublisher, CloudWatchMetricsPublisher>();
   ```
4. No `Function.cs`, após processamento bem-sucedido:
   ```csharp
   var metricsPublisher = _serviceProvider.GetRequiredService<IMetricsPublisher>();
   await metricsPublisher.PublishProcessingDurationAsync(input.Chunk.ChunkId, stopwatch.ElapsedMilliseconds, CancellationToken.None);
   await metricsPublisher.PublishProcessingResultAsync(input.Chunk.ChunkId, output.Status, CancellationToken.None);
   await metricsPublisher.PublishChunkProcessedAsync(CancellationToken.None);
   ```

## Formas de Teste
1. **Teste local:** verificar que logs EMF são gerados corretamente (formato JSON válido)
2. **Mock logger:** capturar logs e validar estrutura EMF (_aws, CloudWatchMetrics, Metrics)
3. **CloudWatch:** após deploy, verificar que métricas aparecem no namespace `VideoProcessing/ChunkWorker`

## Critérios de Aceite da Subtask
- [ ] `IMetricsPublisher` criado com métodos: PublishProcessingDurationAsync, PublishProcessingResultAsync, PublishChunkProcessedAsync
- [ ] `CloudWatchMetricsPublisher` implementado usando EMF (logs estruturados)
- [ ] Métricas publicadas: ProcessingDuration (ms), ProcessingResult (count por status), ChunksProcessed (count total)
- [ ] Namespace customizado: `VideoProcessing/ChunkWorker`
- [ ] Métricas publicadas após processamento bem-sucedido no handler
- [ ] Testes unitários verificam formato EMF correto
- [ ] Métricas visíveis no CloudWatch Metrics (após deploy real)
