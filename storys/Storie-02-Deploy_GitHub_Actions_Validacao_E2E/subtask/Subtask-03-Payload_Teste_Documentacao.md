# Subtask 03: Criar Payload de Teste e Documentar Procedimento de Validação

## Descrição
Criar arquivo JSON de payload de teste (`test-payloads/smoke-payload.json`) que representa input válido do Map da Step Functions para 1 chunk, e documentar no README.md o procedimento completo de validação manual e automatizada do deploy.

## Passos de Implementação
1. Criar pasta `test-payloads/` na raiz do projeto
2. Criar `test-payloads/smoke-payload.json` com estrutura mínima esperada pelo Lambda:
   ```json
   {
     "contractVersion": "1.0",
     "videoId": "test-video-123",
     "chunk": {
       "chunkId": "chunk-0",
       "startSec": 0.0,
       "endSec": 10.0
     },
     "source": {
       "bucket": "video-processing-input",
       "key": "videos/test-video.mp4"
     },
     "output": {
       "manifestBucket": "video-processing-output",
       "manifestPrefix": "manifests/test-video-123"
     }
   }
   ```
3. Adicionar variações opcionais (comentadas ou em arquivos separados):
   - `smoke-payload-with-etag.json`: incluindo `etag` em `source`
   - `smoke-payload-with-frames.json`: incluindo `framesBucket` e `framesPrefix` em `output`
4. Atualizar `README.md` com nova seção "CI/CD e Deploy":
   - **Secrets necessários:** listar AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, AWS_REGION
   - **Como configurar secrets:** instruções para GitHub Secrets
   - **Como disparar pipeline:** push para `main` ou `dev`
   - **Como validar deploy manualmente:**
     ```bash
     bash scripts/validate-deployment.sh \
       --state-machine-arn arn:aws:states:us-east-1:123456789012:stateMachine:VideoProcessorStateMachine \
       --execution-name manual-test-$(date +%s) \
       --payload-file test-payloads/smoke-payload.json \
       --output-bucket video-processing-output \
       --output-prefix manifests/test-video-123/chunk-0/
     ```
   - **Como renovar credenciais AWS Academy:** link para documentação ou passo a passo
5. Adicionar checklist de validação pós-deploy no README:
   - [ ] Pipeline GitHub Actions concluído com sucesso
   - [ ] Script de validação retornou exit code 0
   - [ ] Artefatos presentes no S3
   - [ ] Logs visíveis no CloudWatch

## Formas de Teste
1. **Validação do JSON:** usar `jq` ou ferramenta online para validar sintaxe do payload
2. **Teste de schema:** criar validador JSON Schema para o payload (opcional, mas recomendado)
3. **Teste manual:** executar Step Functions com o payload e confirmar que Lambda processa corretamente

## Critérios de Aceite da Subtask
- [ ] Arquivo `test-payloads/smoke-payload.json` criado com estrutura de input mínima válida
- [ ] JSON bem formado (validado com `jq` ou similar)
- [ ] README.md atualizado com seção "CI/CD e Deploy" completa
- [ ] Documentação inclui: secrets necessários, como disparar pipeline, como validar manualmente
- [ ] Checklist de validação pós-deploy documentado
- [ ] Instruções claras para renovar credenciais temporárias do AWS Academy
