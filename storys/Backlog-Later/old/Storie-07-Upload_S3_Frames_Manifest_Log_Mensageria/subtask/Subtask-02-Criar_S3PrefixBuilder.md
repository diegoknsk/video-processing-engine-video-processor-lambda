# Subtask 02: Criar S3PrefixBuilder para convenção determinística

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Criar serviço `S3PrefixBuilder` que gera prefixos S3 determinísticos baseados em videoId e chunkId, seguindo convenção: `processed/{videoId}/{chunkId}/`.

## Tarefas
1. Criar `src/VideoProcessor.Application/Services/S3PrefixBuilder.cs`
2. Implementar métodos:
   ```csharp
   public static string BuildPrefix(string videoId, string chunkId)
       => $"processed/{videoId}/{chunkId}/";
   
   public static string BuildFrameKey(string videoId, string chunkId, string frameName)
       => $"{BuildPrefix(videoId, chunkId)}{frameName}";
   
   public static string BuildManifestKey(string videoId, string chunkId)
       => $"{BuildPrefix(videoId, chunkId)}manifest.json";
   ```
3. Adicionar validação: videoId e chunkId não vazios
4. Criar testes unitários validando prefixos gerados

## Critérios de Aceite
- [ ] Classe `S3PrefixBuilder` criada (pode ser static)
- [ ] Método `BuildPrefix()` retorna: `processed/{videoId}/{chunkId}/`
- [ ] Método `BuildFrameKey()` retorna: `processed/{videoId}/{chunkId}/{frameName}`
- [ ] Método `BuildManifestKey()` retorna: `processed/{videoId}/{chunkId}/manifest.json`
- [ ] Validação lança exceção se videoId ou chunkId vazios
- [ ] Testes unitários cobrem: prefixo válido, frame key válido, manifest key válido, validação de parâmetros
- [ ] Todos os testes passam

## Notas Técnicas
- Usar string interpolation para clareza
- Convenção escolhida permite listar todos os frames de um chunk: `s3://bucket/processed/{videoId}/{chunkId}/`
- Futuramente pode adicionar prefixo de ambiente: `{env}/processed/...` (não agora)
- Documentar convenção em `docs/S3_CONVENTIONS.md`
