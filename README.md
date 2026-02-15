# Video Processor Lambda

Lambda Worker para processar chunks individuais de vídeo no pipeline da Step Functions. Projeto minimalista com handler puro em .NET 10 (sem AddAWSLambdaHosting).

## Pré-requisitos

- .NET 10 SDK
- AWS CLI (para testes locais com S3)

## Como Buildar

```bash
dotnet build
```

## Como Rodar Testes

```bash
dotnet test
```

## Como Empacotar

Com AWS Lambda Tools instalado (`dotnet tool install -g Amazon.Lambda.Tools`):

```bash
cd src/VideoProcessor.Lambda
dotnet lambda package -o ../../artifacts/VideoProcessor.zip
```

Alternativa (sem Lambda Tools):

```bash
dotnet publish src/VideoProcessor.Lambda -c Release -o publish
cd publish && zip -r ../artifacts/VideoProcessor.zip .
```

## Estrutura de Pastas

| Projeto | Descrição |
|---------|-----------|
| `src/VideoProcessor.Domain` | Entidades e ports (camada de domínio) |
| `src/VideoProcessor.Application` | Use cases e regras de negócio |
| `src/VideoProcessor.Infra` | Implementações (S3, etc.) |
| `src/VideoProcessor.Lambda` | Handler Lambda e bootstrap de DI |
| `tests/VideoProcessor.Tests.Unit` | Testes unitários (xUnit) |
| `tests/VideoProcessor.Tests.Bdd` | Testes BDD (SpecFlow + xUnit) |

## CI/CD e Deploy

### Secrets necessários

Configure os seguintes secrets no GitHub (Settings → Secrets and variables → Actions):

| Secret | Descrição |
|--------|-----------|
| `AWS_ACCESS_KEY_ID` | Access Key da conta AWS |
| `AWS_SECRET_ACCESS_KEY` | Secret Key da conta AWS |
| `AWS_SESSION_TOKEN` | Session Token (obrigatório para credenciais temporárias do AWS Academy) |
| `AWS_REGION` | Região AWS (ex.: `us-east-1`) |

### Como disparar o pipeline

O workflow `.github/workflows/deploy-lambda.yml` é executado automaticamente em **push** para as branches `main` ou `dev`.

### Como validar o deploy manualmente

Após o pipeline concluir, execute o script de validação:

```bash
bash scripts/validate-deployment.sh \
  --state-machine-arn arn:aws:states:us-east-1:123456789012:stateMachine:VideoProcessorStateMachine \
  --execution-name manual-test-$(date +%s) \
  --payload-file test-payloads/smoke-payload.json \
  --output-bucket video-processing-output \
  --output-prefix manifests/test-video-123/chunk-0/
```

Substitua `--state-machine-arn` e `--output-bucket` pelos valores reais da sua conta AWS.

### Renovar credenciais AWS Academy

Credenciais temporárias do AWS Academy expiram. Para renovar:

1. Acesse o AWS Academy e gere novas credenciais temporárias
2. Atualize os secrets `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` e `AWS_SESSION_TOKEN` no GitHub
3. O próximo push disparará o pipeline com as novas credenciais

### Checklist de validação pós-deploy

- [ ] Pipeline GitHub Actions concluído com sucesso
- [ ] Script de validação retornou exit code 0
- [ ] Artefatos `manifest.json` e `done.json` presentes no S3
- [ ] Logs visíveis no CloudWatch com videoId/chunkId esperados

Evidências devem ser salvas em `docs/deploy-validation/` (ver [docs/deploy-validation/README.md](docs/deploy-validation/README.md)).
