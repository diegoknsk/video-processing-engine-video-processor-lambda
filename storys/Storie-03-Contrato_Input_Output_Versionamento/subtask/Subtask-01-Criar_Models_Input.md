# Subtask 01: Criar Models de Input (ChunkProcessorInput e Dependentes)

## Descrição
Criar records C# 13 para representar o input do Lambda: `ChunkProcessorInput` (raiz), `ChunkInfo`, `SourceInfo` e `OutputConfig`, com anotações JSON para System.Text.Json e propriedades opcionais configuradas corretamente.

## Passos de Implementação
1. Criar `src/VideoProcessor.Domain/Models/ChunkInfo.cs`:
   ```csharp
   public record ChunkInfo(
       string ChunkId,
       double StartSec,
       double EndSec
   );
   ```
2. Criar `src/VideoProcessor.Domain/Models/SourceInfo.cs`:
   ```csharp
   public record SourceInfo(
       string Bucket,
       string Key,
       string? Etag = null,
       string? VersionId = null
   );
   ```
3. Criar `src/VideoProcessor.Domain/Models/OutputConfig.cs`:
   ```csharp
   public record OutputConfig(
       string ManifestBucket,
       string ManifestPrefix,
       string? FramesBucket = null,
       string? FramesPrefix = null
   );
   ```
4. Criar `src/VideoProcessor.Domain/Models/ChunkProcessorInput.cs`:
   ```csharp
   public record ChunkProcessorInput(
       string ContractVersion,
       string VideoId,
       ChunkInfo Chunk,
       SourceInfo Source,
       OutputConfig Output,
       string? ExecutionArn = null
   );
   ```
5. Adicionar anotações JSON se necessário (JsonPropertyName) para camelCase:
   ```csharp
   using System.Text.Json.Serialization;
   
   [JsonPropertyName("contractVersion")]
   public string ContractVersion { get; init; }
   ```
   (Ou configurar JsonSerializerOptions com PropertyNamingPolicy = JsonNamingPolicy.CamelCase globalmente)
6. Configurar nullability correta: campos opcionais marcados com `?`

## Formas de Teste
1. **Compilação:** `dotnet build src/VideoProcessor.Domain` compila sem erros
2. **Deserialização manual:** criar teste quick que deserializa JSON de exemplo para `ChunkProcessorInput`
3. **Validação de tipos:** confirmar que campos obrigatórios não permitem null e opcionais permitem

## Critérios de Aceite da Subtask
- [x] Records `ChunkInfo`, `SourceInfo`, `OutputConfig`, `ChunkProcessorInput` criados
- [x] Campos obrigatórios vs opcionais definidos corretamente (Etag, VersionId, ExecutionArn, FramesBucket/Prefix opcionais)
- [x] Anotações JSON configuradas (camelCase se necessário)
- [x] Projeto Domain compila sem warnings de nullability
- [x] Estrutura reflete exatamente o payload esperado do Map da Step Functions
