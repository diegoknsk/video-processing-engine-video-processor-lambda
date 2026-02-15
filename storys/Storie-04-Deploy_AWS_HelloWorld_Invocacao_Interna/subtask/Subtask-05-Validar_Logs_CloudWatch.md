# Subtask 05: Validar logs no CloudWatch e criar guia de invocação

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Acessar CloudWatch Logs, localizar logs da função Lambda, validar que mensagens estão corretas, e criar documentação completa de invocação (`INVOCATION_GUIDE.md`) consolidando tudo.

## Tarefas
1. Acessar AWS Console → CloudWatch → Log groups → `/aws/lambda/video-processor-lambda`
2. Localizar stream mais recente (última invocação)
3. Validar que logs contêm:
   - START RequestId: ...
   - Mensagem: "Hello World invoked at ..."
   - END RequestId: ...
   - REPORT RequestId: ... Duration: ... Memory Used: ...
4. Capturar screenshot dos logs
5. Criar `docs/INVOCATION_GUIDE.md` com:
   - Introdução
   - Invocação via Console (passo-a-passo + screenshots)
   - Invocação via AWS CLI (comandos + exemplos)
   - Como verificar logs no CloudWatch
   - Troubleshooting comum
6. Atualizar `README.md` com link para guia

## Critérios de Aceite
- [ ] Logs aparecem no CloudWatch corretamente
- [ ] Mensagem "Hello World invoked" presente nos logs
- [ ] `docs/INVOCATION_GUIDE.md` criado com todas as seções
- [ ] Screenshots incluídos no guia
- [ ] Troubleshooting documenta: credenciais expiradas, região errada, logs não aparecem (delay ~5s)
- [ ] `README.md` atualizado com seção "Como Invocar Lambda Manualmente" linkando para guia

## Notas Técnicas
- Logs podem ter delay de 5-10 segundos para aparecer
- Se logs não aparecem, verificar se função executou com sucesso (Console → Monitoring → Invocations)
- Log group criado automaticamente na primeira invocação
- Formato de log: CloudWatch usa formato texto simples + metadados AWS
