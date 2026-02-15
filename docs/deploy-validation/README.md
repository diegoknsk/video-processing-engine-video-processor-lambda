# Evidências de Validação do Deploy

Esta pasta deve conter as evidências capturadas após execução do deploy e validação end-to-end (Subtask 04 da Story 02).

## Evidências esperadas

1. **Screenshot do GitHub Actions** — Jobs `build-and-test` e `deploy` com sucesso
2. **Saída do script de validação** — Exit code 0 e logs de sucesso
3. **Screenshot do S3** — Presença de `manifest.json` e `done.json` no prefixo esperado
4. **Logs do CloudWatch** — Entrada do Lambda com `videoId` e `chunkId` esperados

## Como executar a validação completa

1. Configure os secrets no GitHub (ver README principal, seção CI/CD)
2. Faça push para `main` ou `dev` para disparar o pipeline
3. Aguarde conclusão do workflow
4. Execute o script de validação com ARNs e buckets reais
5. Salve as evidências nesta pasta
