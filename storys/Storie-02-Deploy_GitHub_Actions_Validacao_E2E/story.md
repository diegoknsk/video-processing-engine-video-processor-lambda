# Storie-02: Deploy via GitHub Actions + Validação End-to-End (AWS Academy)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como DevOps do projeto, quero configurar pipeline de CI/CD no GitHub Actions usando credenciais temporárias do AWS Academy, para automatizar build, testes, empacotamento e deploy do Lambda, e validar end-to-end que o processador está funcionando integrado com Step Functions e S3.

## Objetivo
Criar workflow GitHub Actions que builda, testa, empacota ZIP e atualiza função Lambda usando credenciais temporárias (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN) armazenadas como secrets, e executar validação pós-deploy via Step Functions + verificação de artefatos no S3 e logs no CloudWatch.

## Escopo Técnico
- **Tecnologias:** GitHub Actions, AWS Lambda, AWS Step Functions, AWS S3, AWS CloudWatch Logs, AWS CLI
- **Arquivos criados:**
  - `.github/workflows/deploy-lambda.yml`
  - `scripts/validate-deployment.sh` (ou .ps1 para Windows)
  - `test-payloads/smoke-payload.json` (payload de teste para Step Functions)
  - Atualização do `README.md` com seção de CI/CD
- **Componentes:** Workflow de CI/CD, scripts de validação, configuração de secrets
- **Pacotes/Dependências:**
  - aws-cli (latest, via GitHub Actions)
  - jq (para parsing de JSON em scripts bash)
  - PowerShell (se usar .ps1)

## Dependências e Riscos (para estimativa)
- **Dependências:** 
  - Storie-01 concluída (projeto bootstrap funcional)
  - Função Lambda já criada manualmente na AWS (primeira vez)
  - Step Functions definida (pode ser state machine simplificada para teste)
  - Bucket S3 criado para output
- **Riscos:**
  - Credenciais temporárias do AWS Academy expiram (mitigar: documentar renovação)
  - Step Functions pode não existir ainda (mitigar: criar state machine mínima de teste)
  - Timeout de execução do Lambda muito baixo (mitigar: configurar 30s+ no Terraform/manual)
- **Pré-condições:**
  - Secrets configurados no GitHub: AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, AWS_REGION
  - Função Lambda criada na AWS (nome: `video-processor-chunk-worker`)
  - Bucket S3 existente para manifestos

## Subtasks
- [Subtask 01: Criar workflow GitHub Actions para build e deploy](./subtask/Subtask-01-Criar_Workflow_Deploy.md)
- [Subtask 02: Implementar script de validação pós-deploy](./subtask/Subtask-02-Script_Validacao_Pos_Deploy.md)
- [Subtask 03: Criar payload de teste e documentar procedimento de validação](./subtask/Subtask-03-Payload_Teste_Documentacao.md)
- [Subtask 04: Executar deploy real e validar end-to-end](./subtask/Subtask-04-Executar_Deploy_Validar_E2E.md)

## Critérios de Aceite da História
- [ ] Workflow `.github/workflows/deploy-lambda.yml` executado com sucesso no GitHub Actions
- [ ] Build compila todos os projetos sem erros
- [ ] Testes (unitários + BDD smoke) passam no pipeline
- [ ] ZIP gerado e função Lambda atualizada via `aws lambda update-function-code`
- [ ] Script de validação executa Step Functions com payload de 1 chunk e confirma sucesso
- [ ] Artefatos esperados (manifest.json, done.json) presentes no S3 após execução
- [ ] Logs do Lambda visíveis no CloudWatch com mensagens esperadas (videoId, chunkId)
- [ ] README.md atualizado com seção "CI/CD e Deploy" documentando secrets necessários e como rodar pipeline
- [ ] Pipeline roda em menos de 5 minutos (sanity check de performance)

## Rastreamento (dev tracking)
- **Início:** 14/02/2026, às 21:24 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
