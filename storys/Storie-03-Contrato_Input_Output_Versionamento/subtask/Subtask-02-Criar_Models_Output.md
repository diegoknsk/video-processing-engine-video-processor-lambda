# Subtask 02: Criar Models de Output (ChunkProcessorOutput e Dependentes)

## Descrição
Criar records C# 13 para representar o output do Lambda: `ChunkProcessorOutput` (raiz), `ManifestInfo`, `ErrorInfo`, com status enum (SUCCEEDED/FAILED) e campos opcionais para error.

## Passos de Implementação
1. Criar `src/VideoProcessor.Domain/Models/ProcessingStatus.cs` (enum):
   ```csharp
   public enum ProcessingStatus
   {
       SUCCEEDED,
       FAILED
   }
   ```
   Adicionar anotação para serializar como string:
   ```csharp
   [JsonConverter(typeof(JsonStringEnumConverter))]
   ```
2. Criar `src/VideoProcessor.Domain/Models/ManifestInfo.cs`:
   ```csharp
   public record ManifestInfo(
       string Bucket,
       string Key
   );
   ```
3. Criar `src/VideoProcessor.Domain/Models/ErrorInfo.cs`:
   ```csharp
   public record ErrorInfo(
       string Type,
       string Message,
       bool Retryable
   );
   ```
4. Criar `src/VideoProcessor.Domain/Models/ChunkProcessorOutput.cs`:
   ```csharp
   public record ChunkProcessorOutput(
       string ChunkId,
       ProcessingStatus Status,
       int FramesCount,
       ManifestInfo? Manifest = null,
       ErrorInfo? Error = null
   );
   ```
   Regras:
   - Se `Status == SUCCEEDED`: `Manifest` obrigatório, `Error` null
   - Se `Status == FAILED`: `Error` obrigatório, `Manifest` null
   (Validação lógica pode ser feita em factory method ou validator)
5. Adicionar anotações JSON para camelCase se necessário

## Formas de Teste
1. **Compilação:** `dotnet build src/VideoProcessor.Domain` compila sem erros
2. **Serialização manual:** criar teste quick que serializa `ChunkProcessorOutput` para JSON e valida estrutura
3. **Enum como string:** verificar que `ProcessingStatus` serializa como "SUCCEEDED"/"FAILED" (não como número)

## Critérios de Aceite da Subtask
- [x] Records `ProcessingStatus`, `ManifestInfo`, `ErrorInfo`, `ChunkProcessorOutput` criados
- [x] Enum `ProcessingStatus` serializa como string (SUCCEEDED/FAILED)
- [x] Campos opcionais: `Manifest` (null em FAILED), `Error` (null em SUCCEEDED)
- [x] Anotações JSON configuradas (camelCase)
- [x] Projeto Domain compila sem warnings
