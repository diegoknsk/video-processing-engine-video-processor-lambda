# Subtask 04: Invocar Lambda via AWS CLI e documentar

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Usar AWS CLI local para invocar a função Lambda, validar resposta, e documentar comandos completos para referência futura.

## Tarefas
1. Validar que AWS CLI está instalado: `aws --version`
2. Validar credenciais: `aws sts get-caller-identity`
3. Invocar Lambda:
   ```bash
   aws lambda invoke \
     --function-name video-processor-lambda \
     --payload '{}' \
     --cli-binary-format raw-in-base64-out \
     response.json
   ```
4. Verificar resposta: `cat response.json`
5. Validar que payload Hello World está correto
6. Documentar comando completo em `docs/INVOCATION_GUIDE.md`

## Critérios de Aceite
- [ ] AWS CLI invoca Lambda com sucesso
- [ ] `response.json` contém payload Hello World esperado
- [ ] Comando documentado com todas as flags necessárias
- [ ] Troubleshooting comum documentado (ex: credenciais expiradas, região errada)
- [ ] Testado em ambiente local (Windows PowerShell)

## Notas Técnicas
- Flag `--cli-binary-format raw-in-base64-out` necessária para AWS CLI v2
- Se erro "Invalid base64", usar payload em arquivo: `--payload file://input.json`
- Região pode ser especificada com `--region us-east-1`
- Para ver logs: `aws logs tail /aws/lambda/video-processor-lambda --follow`
