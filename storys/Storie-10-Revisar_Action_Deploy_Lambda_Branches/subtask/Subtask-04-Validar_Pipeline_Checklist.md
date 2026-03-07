# Subtask 04: Validar pipeline e checklist pós-deploy

## Descrição
Validar que o pipeline completo (build, test, package, deploy) funciona para as branches main e dev, e registrar um checklist pós-deploy para garantir que a Lambda da Story 09 está operacional após cada deploy.

## Passos de implementação
1. Executar o workflow na branch **dev** (push ou workflow_dispatch) e verificar que: checkout → setup .NET → restore → build → test → package → deploy (função dev) concluem sem falha.
2. Executar o workflow na branch **main** (em ambiente controlado ou após merge) e verificar que o deploy de produção conclui sem falha.
3. Elaborar um checklist curto (em `docs/` ou no README) para pós-deploy: (a) função atualizada na AWS; (b) handler correto; (c) invocação de teste com payload `ChunkProcessorInput` retorna 200 e corpo `ChunkProcessorOutput`; (d) se aplicável, conferir logs no CloudWatch.
4. Opcional: adicionar um step no workflow que invoque a função com um payload de smoke após o deploy (dev ou main), falhando o job se a resposta for erro; se não for implementado, deixar o checklist como procedimento manual.

## Formas de teste
- Rodar o pipeline em dev e em main e confirmar que todos os jobs passam.
- Seguir o checklist pós-deploy manualmente após um deploy e registrar se algum item falhou (e ajustar doc ou workflow conforme necessário).

## Critérios de aceite da subtask
- [x] O pipeline (build, test, deploy) executa com sucesso para push em **dev** e em **main** (mesma função `LAMBDA_FUNCTION_NAME`).
- [x] Existe um checklist pós-deploy documentado (função atualizada, handler, invocação de teste, logs).
- [x] Pelo menos uma execução completa do pipeline foi feita e documentada como bem-sucedida (ou eventuais falhas conhecidas documentadas com workaround).
- [x] Nenhuma regressão: o conteúdo da Story 09 (processamento real) continua sendo o que é implantado pela action, sem reverter para Hello World.
