# Subtask 03: Invocar Lambda via Console AWS e documentar

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Acessar o Console AWS, navegar até a função Lambda, usar a funcionalidade "Test" para invocar manualmente, e documentar o processo com screenshots.

## Tarefas
1. Acessar AWS Console → Lambda → Functions → `video-processor-lambda`
2. Clicar em aba "Test"
3. Criar novo test event:
   - Event name: `HelloWorldTest`
   - Payload: `{}`
4. Executar test e validar resposta
5. Capturar screenshot mostrando:
   - Payload de entrada
   - Resposta JSON com Hello World
   - Logs de execução
6. Salvar screenshots em `docs/screenshots/console-invocation.png`

## Critérios de Aceite
- [ ] Lambda invocado com sucesso via Console
- [ ] Resposta contém payload Hello World esperado
- [ ] Logs mostram: request ID, duração, mensagem "Hello World invoked"
- [ ] Screenshot capturado e salvo em `docs/screenshots/`
- [ ] Processo documentado com passo-a-passo

## Notas Técnicas
- Se função não aparecer no Console, verificar região AWS correta
- Resposta esperada: status 200, payload JSON estruturado
- Duração esperada: < 100ms (Hello World é muito rápido)
