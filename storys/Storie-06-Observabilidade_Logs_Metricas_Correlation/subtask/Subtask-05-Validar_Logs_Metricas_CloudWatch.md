# Subtask 05: Validar Logs e Métricas no CloudWatch

## Descrição
Realizar deploy do Lambda atualizado, executar Step Functions com payload de teste, e validar que logs estruturados e métricas customizadas aparecem corretamente no CloudWatch Logs e CloudWatch Metrics.

## Passos de Implementação
1. Deploy via GitHub Actions (usar workflow da Storie-02)
2. Executar Step Functions com payload de teste:
   ```bash
   bash scripts/validate-deployment.sh \
     --state-machine-arn <ARN> \
     --execution-name obs-test-$(date +%s) \
     --payload-file test-payloads/smoke-payload.json \
     --output-bucket <BUCKET> \
     --output-prefix manifests/test-video-123/chunk-0/
   ```
3. Validar logs no CloudWatch Logs:
   - Abrir log group `/aws/lambda/video-processor-chunk-worker`
   - Verificar que logs incluem campos: videoId, chunkId, executionArn, correlationId
   - Verificar fluxo completo: início → validação → idempotência → S3 writes → sucesso
4. Criar queries no CloudWatch Logs Insights:
   - **Query 1: Logs por videoId**
     ```
     fields @timestamp, @message, videoId, chunkId, correlationId
     | filter videoId = "test-video-123"
     | sort @timestamp desc
     ```
   - **Query 2: Erros por chunkId**
     ```
     fields @timestamp, @message, chunkId, errorType, retryable
     | filter @message like /Error/
     | sort @timestamp desc
     ```
5. Validar métricas no CloudWatch Metrics:
   - Navegar para namespace `VideoProcessing/ChunkWorker`
   - Verificar métricas: ProcessingDuration, ProcessingResult (dimensão: Status), ChunksProcessed
   - Confirmar que valores batem com execuções (ex.: 1 chunk processado = 1 count em ChunksProcessed)
6. Capturar screenshots:
   - Logs estruturados com correlation fields
   - Query Logs Insights retornando resultados
   - Métricas customizadas no CloudWatch Metrics
7. Documentar em `docs/observability/validation-results.md`

## Formas de Teste
1. **Logs:** verificar visualmente que logs aparecem estruturados e completos
2. **Queries:** executar queries Logs Insights e confirmar filtros funcionam
3. **Métricas:** verificar que gráficos aparecem no CloudWatch Metrics

## Critérios de Aceite da Subtask
- [ ] Deploy realizado com código de observabilidade
- [ ] Step Functions executada com sucesso
- [ ] Logs aparecem no CloudWatch Logs com campos: videoId, chunkId, executionArn, correlationId, @timestamp, @message
- [ ] Query Logs Insights filtra logs por videoId corretamente
- [ ] Query Logs Insights filtra erros corretamente
- [ ] Métricas customizadas aparecem no namespace `VideoProcessing/ChunkWorker`
- [ ] Métrica ProcessingDuration mostra valor em ms (> 0)
- [ ] Métrica ProcessingResult mostra count = 1 com dimensão Status=SUCCEEDED
- [ ] Métrica ChunksProcessed mostra count = 1
- [ ] Screenshots capturados e documentados em `docs/observability/validation-results.md`
