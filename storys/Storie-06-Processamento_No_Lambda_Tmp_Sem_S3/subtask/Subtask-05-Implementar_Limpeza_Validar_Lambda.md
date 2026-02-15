# Subtask 05: Implementar limpeza /tmp e validar execução no Lambda

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Implementar limpeza automática de arquivos temporários em /tmp ao final do processamento (sucesso ou falha), e validar execução completa no Lambda AWS.

## Tarefas
1. Adicionar bloco `finally` no handler:
   ```csharp
   try
   {
       // processar vídeo
   }
   finally
   {
       CleanupTempFiles("/tmp/input.mp4", "/tmp/frames");
   }
   ```
2. Implementar método `CleanupTempFiles()`:
   - Deletar arquivo de vídeo se existe
   - Deletar pasta de frames recursivamente
   - Logar arquivos removidos
   - Capturar exceções (não falhar se cleanup falhar)
3. Testar execução completa na AWS:
   - Invocar Lambda via Console
   - Validar resposta JSON
   - Verificar logs no CloudWatch
   - Confirmar que frames foram gerados (via log count)
4. Testar cenário de falha controlada (timeout simulado)

## Critérios de Aceite
- [ ] Método `CleanupTempFiles()` implementado
- [ ] Limpeza executada em bloco `finally` (sempre executa)
- [ ] Log mostra: "Vídeo temporário removido", "Pasta de frames removida"
- [ ] Se cleanup falhar, apenas log warning (não interrompe)
- [ ] Lambda executado na AWS com sucesso
- [ ] Resposta JSON retornada corretamente
- [ ] Logs CloudWatch mostram fluxo completo: início → processamento → frames gerados → limpeza
- [ ] Falha controlada se limites excedidos: log error + status FAILED

## Notas Técnicas
- Usar `Directory.Delete(path, recursive: true)` para pasta
- Usar `File.Delete(path)` para arquivo
- Capturar exceções de I/O no cleanup
- /tmp é limpo entre invocações "cold start", mas não em "warm start"
- Sempre limpar explicitamente para economizar espaço
