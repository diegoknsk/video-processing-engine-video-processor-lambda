# Subtask 03: Invocar Lambda via Console AWS e documentar

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Acessar o Console AWS, navegar até a função Lambda, usar a funcionalidade "Test" para invocar manualmente, e documentar o processo com screenshots.

## Tarefas
1. Criar arquivo `tests/payloads/hello-world-test.json` com payload de exemplo:
   ```json
   {
     "message": "Test invocation from Console",
     "testId": "test-001"
   }
   ```
2. Acessar AWS Console → Lambda → Functions → `video-processor-lambda`
3. Clicar em aba "Test"
4. Criar novo test event:
   - Event name: `HelloWorldTest`
   - Payload: copiar conteúdo de `tests/payloads/hello-world-test.json`
5. Executar test e validar resposta
6. Capturar screenshot mostrando:
   - Payload de entrada
   - Resposta JSON com Hello World
   - Logs de execução
7. Salvar screenshots em `docs/screenshots/console-invocation.png`

## Critérios de Aceite
- [ ] Arquivo `tests/payloads/hello-world-test.json` criado com payload de exemplo
- [ ] Lambda invocado com sucesso via Console usando payload do arquivo
- [ ] Resposta contém payload Hello World esperado
- [ ] Logs mostram: request ID, duração, mensagem "Hello World invoked"
- [ ] Screenshot capturado e salvo em `docs/screenshots/`
- [ ] Processo documentado com passo-a-passo

## Notas Técnicas
- Se função não aparecer no Console, verificar região AWS correta
- Resposta esperada: status 200, payload JSON estruturado
- Duração esperada: < 100ms (Hello World é muito rápido)
