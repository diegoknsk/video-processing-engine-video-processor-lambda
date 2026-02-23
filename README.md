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
| `src/VideoProcessor.CLI` | Aplicação console para processamento local de vídeo (extração de frames) |
| `tests/VideoProcessor.Tests.Unit` | Testes unitários (xUnit) |
| `tests/VideoProcessor.Tests.Bdd` | Testes BDD (SpecFlow + xUnit) |

## Processamento Local de Vídeo

É possível testar a extração de frames localmente (Windows) antes de rodar no Lambda.

### Pré-requisitos

- .NET 10 SDK
- FFmpeg: na primeira execução da CLI, o binário é baixado automaticamente para `%USERPROFILE%\.ffmpeg` (via Xabe.FFmpeg.Downloader). Alternativamente, instale o FFmpeg manualmente e configure o `PATH` ou o caminho dos executáveis.

### Como executar

```bash
dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output output/frames
```

- `--video`: caminho do arquivo de vídeo (ex.: `sample.mp4`)
- `--interval`: intervalo em segundos entre cada frame (ex.: 20 = um frame a cada 20 s)
- `--output`: pasta onde os frames serão salvos (ex.: `output/frames`)

Os frames são gerados com nomes determinísticos: `frame_0001_0s.jpg`, `frame_0002_20s.jpg`, etc. A mesma duração e o mesmo intervalo sempre geram a mesma quantidade de frames.

## Como Invocar Lambda Manualmente

Para testar e validar o Lambda após o deploy, você pode invocá-lo manualmente de duas formas:

### 1. Via Console AWS

1. Acesse o [AWS Console](https://console.aws.amazon.com/) → **Lambda** → **Functions**
2. Selecione a função `video-processor-chunk-worker`
3. Clique na aba **Test**
4. Crie um novo test event com o payload de `tests/payloads/hello-world-test.json`
5. Clique em **Test** e valide a resposta

**Resposta esperada:**
```json
{
  "message": "Hello World from Video Processor Lambda",
  "version": "1.0.0",
  "timestamp": "2026-02-15T17:14:00.0000000Z",
  "environment": "dev"
}
```

### 2. Via AWS CLI

```bash
aws lambda invoke \
  --function-name video-processor-chunk-worker \
  --payload '{}' \
  --cli-binary-format raw-in-base64-out \
  response.json

cat response.json
```

**Para mais detalhes**, consulte o [Guia de Invocação](docs/INVOCATION_GUIDE.md) com:
- Passo-a-passo completo (Console + CLI)
- Como verificar logs no CloudWatch
- Troubleshooting comum
- Screenshots e exemplos

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
