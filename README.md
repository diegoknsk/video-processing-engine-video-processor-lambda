# Video Processor Lambda

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=diegoknsk_video-processing-engine-video-processor-lambda&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=diegoknsk_video-processing-engine-video-processor-lambda)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=diegoknsk_video-processing-engine-video-processor-lambda&metric=coverage)](https://sonarcloud.io/summary/new_code?id=diegoknsk_video-processing-engine-video-processor-lambda)

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
dotnet lambda package -o artifacts/VideoProcessor.zip -c Release -pl src/InterfacesExternas/VideoProcessor.Lambda
```

Alternativa (sem Lambda Tools):

```bash
dotnet publish src/InterfacesExternas/VideoProcessor.Lambda -c Release -o publish
cd publish && zip -r ../artifacts/VideoProcessor.zip .
```

## Estrutura de Pastas

| Projeto | Descrição |
|---------|-----------|
| `src/Core/VideoProcessor.Domain` | Entidades e ports (camada de domínio) |
| `src/Core/VideoProcessor.Application` | Use cases e regras de negócio |
| `src/Infra/VideoProcessor.Infra` | Implementações (S3, etc.) |
| `src/InterfacesExternas/VideoProcessor.Lambda` | Handler Lambda e bootstrap de DI |
| `src/InterfacesExternas/VideoProcessor.CLI` | Aplicação console para processamento local de vídeo (extração de frames) |
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
- `--start` (opcional): tempo de início do trecho em segundos
- `--end` (opcional): tempo de fim do trecho em segundos. Quando informados, apenas o trecho [start, end] é processado.

Exemplo para processar apenas o primeiro minuto (0s a 59s) e depois o segundo minuto (60s a 119s) em um vídeo longo:

```bash
dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output out/ --start 0 --end 59
dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output out/ --start 60 --end 119
```

Os frames são gerados com nomes determinísticos: `frame_0001_0s.jpg`, `frame_0002_20s.jpg`, etc. A mesma duração e o mesmo intervalo sempre geram a mesma quantidade de frames.

## Como Invocar Lambda Manualmente

Para testar e validar o Lambda após o deploy, use um payload `ChunkProcessorInput` válido (videoId, chunk, bucket, etc.). Exemplos em [docs/payload-examples.md](docs/payload-examples.md).

### Via AWS CLI

```bash
aws lambda invoke \
  --function-name video-processor-chunk-worker \
  --payload file://payloads/chunk-input.json \
  --cli-binary-format raw-in-base64-out \
  response.json

cat response.json
```

**Para mais detalhes**, consulte o [Guia de Invocação](docs/INVOCATION_GUIDE.md) e [Exemplos de payload](docs/payload-examples.md).

## CI/CD e Deploy

O workflow `.github/workflows/deploy-lambda.yml` faz build, testes e deploy da Lambda de processamento de chunks (Story 09).

### Estratégia de branches

- **main** e **dev** → deploy automático para a **mesma** função Lambda (variável `LAMBDA_FUNCTION_NAME`, padrão: `video-processor-chunk-worker`).

Push em outras branches executa apenas build e testes (sem deploy).

### Secrets e variáveis

Configure no GitHub em **Settings → Secrets and variables → Actions**:

| Tipo     | Nome                  | Obrigatório | Descrição |
|----------|------------------------|-------------|-----------|
| **Secret** | `AWS_ACCESS_KEY_ID`    | Sim (deploy) | Credencial AWS |
| **Secret** | `AWS_SECRET_ACCESS_KEY`| Sim (deploy) | Credencial AWS |
| **Secret** | `AWS_SESSION_TOKEN`   | Sim (deploy) | Token de sessão AWS |
| **Secret** | `AWS_REGION`           | Sim (deploy) | Região AWS (ex.: `us-east-1`) |
| **Secret** | `SONAR_TOKEN`          | Sim (SonarCloud) | Token em [sonarcloud.io/account/security](https://sonarcloud.io/account/security) para análise no PR/main |
| **Variable** | `LAMBDA_FUNCTION_NAME` | Não | Nome da função Lambda; padrão: `video-processor-chunk-worker` |

SonarCloud usa projeto fixo `diegoknsk_video-processing-engine-video-processor-lambda` e organização `diegoknsk` no workflow; não é necessário configurar variables para isso.

### FFmpeg na Lambda

A função usa FFmpeg (Xabe.FFmpeg). O deploy **não** empacota o binário; é necessário anexar um **Lambda Layer** com FFmpeg à função ou configurar a variável `FFMPEG_PATH`. Detalhes em [docs/deploy-lambda-video-processor.md](docs/deploy-lambda-video-processor.md).

### Checklist pós-deploy

- [ ] Pipeline concluído com sucesso (build, test, deploy)
- [ ] Função atualizada na AWS (Last modified)
- [ ] Handler: `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler`
- [ ] Invocação com payload `ChunkProcessorInput` retorna resposta coerente com `ChunkProcessorOutput`
- [ ] Logs no CloudWatch sem erro de FFmpeg (se o layer estiver configurado)

Documentação completa do deploy, variáveis e checklist: [docs/deploy-lambda-video-processor.md](docs/deploy-lambda-video-processor.md).
