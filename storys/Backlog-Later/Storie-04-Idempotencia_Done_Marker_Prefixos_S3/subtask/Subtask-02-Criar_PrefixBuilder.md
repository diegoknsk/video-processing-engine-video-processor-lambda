# Subtask 02: Criar PrefixBuilder para Convenção de Prefixos Determinística

## Descrição
Criar serviço `PrefixBuilder` que gera prefixos S3 determinísticos no formato `{manifestPrefix}/{chunkId}/` para artefatos (manifest.json, done.json), e documentar convenção.

## Passos de Implementação
1. Criar `src/VideoProcessor.Application/Services/PrefixBuilder.cs`:
   ```csharp
   public interface IPrefixBuilder
   {
       string BuildChunkPrefix(string manifestPrefix, string chunkId);
       string BuildManifestKey(string manifestPrefix, string chunkId);
       string BuildDoneMarkerKey(string manifestPrefix, string chunkId);
   }
   
   public class PrefixBuilder : IPrefixBuilder
   {
       public string BuildChunkPrefix(string manifestPrefix, string chunkId)
       {
           // Remove trailing slash se existir
           var prefix = manifestPrefix.TrimEnd('/');
           return $"{prefix}/{chunkId}/";
       }
       
       public string BuildManifestKey(string manifestPrefix, string chunkId)
       {
           return $"{BuildChunkPrefix(manifestPrefix, chunkId)}manifest.json";
       }
       
       public string BuildDoneMarkerKey(string manifestPrefix, string chunkId)
       {
           return $"{BuildChunkPrefix(manifestPrefix, chunkId)}done.json";
       }
   }
   ```
2. Registrar no DI:
   ```csharp
   services.AddSingleton<IPrefixBuilder, PrefixBuilder>();
   ```
3. Documentar convenção em `docs/S3_CONVENTIONS.md`:
   - Estrutura: `{manifestPrefix}/{chunkId}/manifest.json` e `{manifestPrefix}/{chunkId}/done.json`
   - Exemplo: `manifestPrefix = "manifests/video-123"`, `chunkId = "chunk-0"` → `manifests/video-123/chunk-0/manifest.json`
   - Trailing slashes: sempre normalizados (removidos do input, adicionados internamente)

## Formas de Teste
1. **Teste unitário:** `BuildManifestKey("manifests/video-123", "chunk-0")` retorna `"manifests/video-123/chunk-0/manifest.json"`
2. **Teste de normalização:** `BuildManifestKey("manifests/video-123/", "chunk-0")` (com trailing slash) retorna mesmo resultado
3. **Teste de done marker:** `BuildDoneMarkerKey` gera chave correta

## Critérios de Aceite da Subtask
- [ ] Interface `IPrefixBuilder` e implementação `PrefixBuilder` criados
- [ ] Método `BuildChunkPrefix` normaliza trailing slashes
- [ ] Métodos `BuildManifestKey` e `BuildDoneMarkerKey` retornam chaves corretas
- [ ] Convenção documentada em `docs/S3_CONVENTIONS.md`
- [ ] Testes unitários cobrem: prefixo normal, prefixo com trailing slash, manifest key, done key
- [ ] `IPrefixBuilder` registrado no DI
