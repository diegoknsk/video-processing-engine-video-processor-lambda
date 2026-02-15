# Subtask 01: Criar CorrelationContext para Propagar videoId/chunkId/executionArn

## Descrição
Criar classe `CorrelationContext` que armazena videoId, chunkId, executionArn e correlationId (GUID), e configurar propagação via `AsyncLocal` ou ILogger scope para que todos os logs incluam esse contexto automaticamente.

## Passos de Implementação
1. Criar `src/VideoProcessor.Application/Services/CorrelationContext.cs`:
   ```csharp
   public class CorrelationContext
   {
       private static readonly AsyncLocal<CorrelationContext?> _current = new();
       
       public static CorrelationContext? Current
       {
           get => _current.Value;
           set => _current.Value = value;
       }
       
       public string VideoId { get; init; } = string.Empty;
       public string ChunkId { get; init; } = string.Empty;
       public string? ExecutionArn { get; init; }
       public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
       
       public Dictionary<string, object> ToDictionary() => new()
       {
           ["videoId"] = VideoId,
           ["chunkId"] = ChunkId,
           ["executionArn"] = ExecutionArn ?? "N/A",
           ["correlationId"] = CorrelationId
       };
   }
   ```
2. No `Function.cs`, após deserializar input, criar e configurar context:
   ```csharp
   var input = JsonSerializer.Deserialize<ChunkProcessorInput>(inputJson, _jsonOptions);
   
   CorrelationContext.Current = new CorrelationContext
   {
       VideoId = input.VideoId,
       ChunkId = input.Chunk.ChunkId,
       ExecutionArn = input.ExecutionArn,
       CorrelationId = Guid.NewGuid().ToString()
   };
   
   context.Logger.LogInformation($"Starting chunk processing. CorrelationId={CorrelationContext.Current.CorrelationId}");
   ```
3. Adicionar using: `using System.Threading;`

## Formas de Teste
1. **Teste unitário:** criar `CorrelationContext`, verificar que propriedades são setadas corretamente
2. **Teste de AsyncLocal:** criar context em uma thread, verificar que está acessível via `Current`
3. **Teste de serialização:** `ToDictionary()` retorna dicionário com todas as chaves esperadas

## Critérios de Aceite da Subtask
- [ ] `CorrelationContext` criado com propriedades: VideoId, ChunkId, ExecutionArn, CorrelationId
- [ ] Context propagado via `AsyncLocal` (acessível via `CorrelationContext.Current`)
- [ ] Método `ToDictionary()` serializa context para dicionário (útil para logs estruturados)
- [ ] Handler popula context no início do processamento
- [ ] Testes unitários cobrem: criação de context, acesso via Current, serialização
