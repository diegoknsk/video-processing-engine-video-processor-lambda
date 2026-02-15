# Subtask 05: Criar Guia de Troubleshooting e Runbook Operacional

## Descrição
Criar guia de troubleshooting listando problemas comuns e soluções, e runbook operacional documentando como monitorar, investigar falhas e executar operações de manutenção do Lambda Worker.

## Passos de Implementação
1. Criar `docs/TROUBLESHOOTING.md`:
   ```markdown
   # Guia de Troubleshooting — Lambda Video Processor
   
   ## Problemas Comuns
   
   ### 1. Credenciais AWS Academy Expiradas
   **Sintoma:** Pipeline GitHub Actions falha com erro "The security token included in the request is expired"
   
   **Causa:** Credenciais temporárias do AWS Academy expiraram (válidas por algumas horas)
   
   **Solução:**
   1. Acessar AWS Academy Learner Lab
   2. Clicar em "AWS Details" → "AWS CLI"
   3. Copiar novas credenciais (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN)
   4. Atualizar GitHub Secrets
   
   ---
   
   ### 2. Lambda Retorna FAILED com "ValidationError"
   **Sintoma:** Step Functions mostra chunk FAILED; log mostra "Input validation failed"
   
   **Causa:** Input do Map não está no formato esperado (campos obrigatórios faltando, tipos incorretos)
   
   **Solução:**
   1. Verificar logs CloudWatch do Lambda (filtrar por videoId/chunkId)
   2. Identificar campo inválido na mensagem de erro (ex.: "VideoId is required")
   3. Corrigir payload de entrada da Step Functions (state machine definition)
   4. Validar payload com schema JSON (test-payloads/)
   
   ---
   
   ### 3. Done Marker Órfão (chunk nunca completa)
   **Sintoma:** Chunk marca como SUCCEEDED mas manifest.json não existe; reprocessamento continua retornando idempotente
   
   **Causa:** Lambda escreveu done.json mas falhou antes de escrever manifest.json (cenário raro)
   
   **Solução:**
   1. Identificar chunk órfão (done.json existe, manifest.json não)
   2. Deletar done.json manualmente:
      ```bash
      aws s3 rm s3://bucket/prefix/chunkId/done.json
      ```
   3. Re-executar Step Functions para o chunk
   
   ---
   
   ### 4. Logs Não Aparecem no CloudWatch
   **Sintoma:** Lambda executa mas logs não aparecem em /aws/lambda/video-processor-chunk-worker
   
   **Causa:** Permissão de logs faltando ou log group não existe
   
   **Solução:**
   1. Verificar role do Lambda tem permissão `logs:CreateLogStream` e `logs:PutLogEvents`
   2. Criar log group manualmente se não existir:
      ```bash
      aws logs create-log-group --log-group-name /aws/lambda/video-processor-chunk-worker
      ```
   3. Re-executar Lambda
   
   ---
   
   ### 5. Timeout de Lambda
   **Sintoma:** Lambda falha com "Task timed out after X seconds"
   
   **Causa:** Processamento leva mais tempo que timeout configurado (padrão: 3s)
   
   **Solução:**
   1. Aumentar timeout do Lambda:
      ```bash
      aws lambda update-function-configuration \
        --function-name video-processor-chunk-worker \
        --timeout 30
      ```
   2. Monitorar duração média via métrica ProcessingDuration
   3. Se timeout persistir, investigar operação lenta (S3, rede)
   ```
2. Criar `docs/RUNBOOK.md`:
   ```markdown
   # Runbook Operacional — Lambda Video Processor
   
   ## Monitoramento
   
   ### Métricas Chave
   - **ProcessingDuration:** duração média/max de processamento (target: < 5s)
   - **ProcessingResult:** count de SUCCEEDED vs FAILED (target: > 95% success)
   - **ChunksProcessed:** volume total processado
   - **Lambda Errors:** invocations failed (CloudWatch Lambda Metrics)
   
   ### Dashboards CloudWatch
   1. Acessar CloudWatch → Dashboards
   2. Criar dashboard "VideoProcessor" com widgets:
      - ProcessingDuration (line chart)
      - ProcessingResult by Status (bar chart)
      - ChunksProcessed (counter)
      - Lambda Errors (line chart)
   
   ### Alertas Recomendados
   - **Alta taxa de falha:** ProcessingResult (Status=FAILED) > 10% em 5min
   - **Duração anormal:** ProcessingDuration > 10s por 3 datapoints consecutivos
   - **Lambda errors:** Lambda Errors > 5 em 5min
   
   ---
   
   ## Investigação de Falhas
   
   ### Passo 1: Identificar Chunks com Falha
   Query CloudWatch Logs Insights:
   ```
   fields @timestamp, videoId, chunkId, status, errorType, retryable
   | filter status = "FAILED"
   | sort @timestamp desc
   | limit 50
   ```
   
   ### Passo 2: Analisar Logs do Chunk
   Query por chunkId específico:
   ```
   fields @timestamp, @message
   | filter chunkId = "chunk-X"
   | sort @timestamp asc
   ```
   
   ### Passo 3: Verificar Artefatos S3
   ```bash
   aws s3 ls s3://bucket/prefix/chunkId/ --recursive
   ```
   Verificar presença de manifest.json e done.json
   
   ### Passo 4: Classificar Erro
   - **ValidationError:** corrigir payload de entrada; não reprocessar
   - **S3Error (retryable):** verificar permissões S3 e status do serviço
   - **UnexpectedError:** investigar stack trace nos logs
   
   ---
   
   ## Operações de Manutenção
   
   ### Forçar Reprocessamento de Chunk
   1. Deletar done marker:
      ```bash
      aws s3 rm s3://bucket/prefix/chunkId/done.json
      ```
   2. Re-executar Step Functions com payload do chunk
   
   ### Limpar Artefatos de Teste
   ```bash
   aws s3 rm s3://bucket/manifests/test-video-123/ --recursive
   ```
   
   ### Atualizar Lambda com Nova Versão
   1. Deploy via GitHub Actions (push para main)
   2. Verificar CloudWatch Logs para warm-up bem-sucedido
   3. Executar smoke test (validação end-to-end)
   
   ### Rollback de Versão
   ```bash
   aws lambda update-function-code \
     --function-name video-processor-chunk-worker \
     --s3-bucket deploy-bucket \
     --s3-key previous-version.zip
   ```
   ```

## Formas de Teste
1. **Revisão de conteúdo:** verificar que problemas listados são realistas e soluções viáveis
2. **Validação técnica:** confirmar comandos AWS CLI estão corretos
3. **Completude:** garantir que cobre cenários comuns de produção

## Critérios de Aceite da Subtask
- [ ] `docs/TROUBLESHOOTING.md` criado com ≥ 5 problemas comuns e soluções
- [ ] Problemas incluem: credenciais expiradas, validação falha, done marker órfão, logs ausentes, timeout
- [ ] `docs/RUNBOOK.md` criado com seções: Monitoramento, Investigação de Falhas, Operações de Manutenção
- [ ] Runbook documenta: métricas chave, dashboards, alertas, queries Logs Insights, comandos AWS CLI para operações
- [ ] Todos os comandos AWS CLI testados e funcionais
- [ ] Documentos linkados no README (seção Operação)
