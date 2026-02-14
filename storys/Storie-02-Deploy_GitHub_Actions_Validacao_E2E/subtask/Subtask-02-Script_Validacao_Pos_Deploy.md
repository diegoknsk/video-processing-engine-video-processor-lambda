# Subtask 02: Implementar Script de Validação Pós-Deploy

## Descrição
Criar script bash (ou PowerShell) que executa validação end-to-end após deploy: invoca Step Functions com payload de teste, aguarda conclusão, verifica se artefatos (manifest.json, done.json) foram criados no S3, e consulta logs no CloudWatch.

## Passos de Implementação
1. Criar `scripts/validate-deployment.sh` (ou `validate-deployment.ps1` para Windows)
2. Script deve receber argumentos:
   - `--state-machine-arn`: ARN da Step Functions
   - `--execution-name`: nome da execução (ex.: `test-deploy-$(date +%s)`)
   - `--payload-file`: caminho para JSON de payload (ex.: `test-payloads/smoke-payload.json`)
   - `--output-bucket`: bucket onde esperar manifest/done
   - `--output-prefix`: prefixo no bucket (ex.: `test-video-123/chunk-0/`)
3. Implementar lógica:
   - **Passo 1:** Iniciar execução da Step Functions:
     ```bash
     EXEC_ARN=$(aws stepfunctions start-execution \
       --state-machine-arn $STATE_MACHINE_ARN \
       --name $EXECUTION_NAME \
       --input file://$PAYLOAD_FILE \
       --query 'executionArn' --output text)
     ```
   - **Passo 2:** Poll status até conclusão (max 2 minutos):
     ```bash
     while true; do
       STATUS=$(aws stepfunctions describe-execution --execution-arn $EXEC_ARN --query 'status' --output text)
       if [ "$STATUS" == "SUCCEEDED" ]; then break; fi
       if [ "$STATUS" == "FAILED" ]; then echo "FAILED"; exit 1; fi
       sleep 5
     done
     ```
   - **Passo 3:** Verificar artefatos no S3:
     ```bash
     aws s3 ls s3://$OUTPUT_BUCKET/$OUTPUT_PREFIX/manifest.json || exit 1
     aws s3 ls s3://$OUTPUT_BUCKET/$OUTPUT_PREFIX/done.json || exit 1
     ```
   - **Passo 4:** Buscar logs do Lambda no CloudWatch (últimos 5 minutos):
     ```bash
     aws logs tail /aws/lambda/video-processor-chunk-worker --since 5m
     ```
4. Adicionar validações:
   - Verificar se videoId e chunkId aparecem nos logs
   - Confirmar que manifest.json contém estrutura esperada (fazer `aws s3 cp` e parsear com `jq`)
5. Script retorna exit code 0 em sucesso, 1 em falha

## Formas de Teste
1. **Execução manual:** rodar script localmente passando ARNs/buckets reais
2. **Dry-run:** testar cada comando AWS CLI isoladamente
3. **Teste de erro:** simular falha (ex.: bucket inexistente) e verificar que script falha corretamente

## Critérios de Aceite da Subtask
- [ ] Script `validate-deployment.sh` (ou .ps1) criado e executável
- [ ] Script inicia execução da Step Functions e aguarda conclusão
- [ ] Script verifica presença de `manifest.json` e `done.json` no S3
- [ ] Script consulta logs do CloudWatch e confirma presença de videoId/chunkId
- [ ] Script retorna exit code 0 em sucesso, 1 em qualquer falha
- [ ] Timeout configurado (script falha se Step Functions não concluir em 2 minutos)
