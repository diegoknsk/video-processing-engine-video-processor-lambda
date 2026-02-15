# Subtask 04: Executar Deploy Real e Validar End-to-End

## Descrição
Realizar deploy real via GitHub Actions, executar script de validação, confirmar que Lambda processou chunk mockado com sucesso, e documentar evidências (screenshots, logs, artefatos S3).

## Passos de Implementação
1. Configurar secrets no GitHub (Settings → Secrets and variables → Actions):
   - AWS_ACCESS_KEY_ID
   - AWS_SECRET_ACCESS_KEY
   - AWS_SESSION_TOKEN
   - AWS_REGION (ex.: us-east-1)
2. Fazer commit e push de todo o código para branch `dev` ou `main` para disparar pipeline
3. Acompanhar execução do workflow no GitHub Actions:
   - Verificar que job `build-and-test` passa
   - Verificar que job `deploy` atualiza Lambda com sucesso
4. Após deploy concluído, executar script de validação localmente:
   ```bash
   bash scripts/validate-deployment.sh \
     --state-machine-arn <ARN_REAL> \
     --execution-name smoke-test-$(date +%s) \
     --payload-file test-payloads/smoke-payload.json \
     --output-bucket <BUCKET_REAL> \
     --output-prefix manifests/test-video-123/chunk-0/
   ```
5. Capturar evidências:
   - Screenshot do GitHub Actions mostrando jobs com sucesso
   - Saída do script de validação mostrando exit code 0
   - Screenshot do S3 mostrando `manifest.json` e `done.json`
   - Logs do CloudWatch mostrando entrada do Lambda com videoId/chunkId
6. Adicionar evidências em pasta `docs/deploy-validation/` (screenshots e logs)
7. Atualizar README.md ou criar `DEPLOY.md` com link para evidências

## Formas de Teste
1. **Pipeline completo:** verificar que GitHub Actions completa sem erros
2. **Validação E2E:** script retorna 0 e artefatos confirmados no S3
3. **Logs:** CloudWatch mostra execução do Lambda com input correto

## Critérios de Aceite da Subtask
- [ ] Secrets configurados no GitHub e pipeline executado com sucesso
- [ ] Função Lambda atualizada (verificar última modificação no console AWS ou via CLI)
- [ ] Script de validação retorna exit code 0
- [ ] Arquivos `manifest.json` e `done.json` presentes no S3 no prefixo esperado
- [ ] Logs do CloudWatch mostram execução do Lambda com videoId="test-video-123" e chunkId="chunk-0"
- [ ] Evidências capturadas e salvas em `docs/deploy-validation/`
- [ ] Deploy completo (do push ao artefatos no S3) ocorre em menos de 5 minutos
