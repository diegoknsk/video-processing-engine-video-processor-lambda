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

## Arquitetura

O projeto segue **Clean Architecture** com handler Lambda puro (sem `AddAWSLambdaHosting`), organizado em três camadas físicas:

```
src/
  Core/
    VideoProcessor.Domain        # Entidades, ports, exceções — sem dependências externas
    VideoProcessor.Application   # Use cases, serviços (FFmpeg, validação)
  Infra/
    VideoProcessor.Infra         # Implementação concreta: S3 via AWSSDK
  InterfacesExternas/
    VideoProcessor.Lambda        # Entry point Lambda: bootstrap DI, handler
    VideoProcessor.CLI           # Console app para testes locais
tests/
  VideoProcessor.Tests.Unit      # xUnit
  VideoProcessor.Tests.Bdd       # SpecFlow + xUnit
```

### Fluxo de processamento

```
Step Functions
     │
     ▼
Lambda Handler (Function.FunctionHandler)
     │  ChunkProcessorInput
     ▼
ProcessChunkUseCase.ExecuteAsync
     ├─ IS3VideoStorage.DownloadToTempAsync   (Infra → S3)
     ├─ IVideoFrameExtractor.ExtractFramesAsync  (Application → FFmpeg)
     └─ IS3VideoStorage.UploadFramesAsync     (Infra → S3)
     │  ChunkProcessorOutput
     ▼
Lambda Handler → serializa JSON e retorna
```

### Contrato de entrada (`ChunkProcessorInput`)

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `contractVersion` | `string` | Versão do contrato (suportado: `"1.0"`) |
| `videoId` | `string` | Identificador único do vídeo |
| `chunk.chunkId` | `string` | Identificador do chunk |
| `chunk.startSec` | `double` | Segundo de início do chunk |
| `chunk.endSec` | `double` | Segundo de fim do chunk |
| `chunk.intervalSec` | `int` | Intervalo entre frames em segundos (padrão: `1`) |
| `source.bucket` | `string` | Bucket S3 de origem (arquivo de vídeo) |
| `source.key` | `string` | Chave S3 do arquivo de vídeo |
| `output.manifestBucket` | `string` | Bucket S3 para o manifest |
| `output.manifestPrefix` | `string` | Prefix S3 para o manifest |
| `output.framesBucket` | `string` | Bucket S3 para os frames (opcional, usa `manifestBucket` se omitido) |
| `output.framesPrefix` | `string` | Prefix S3 para os frames (opcional, usa `manifestPrefix` se omitido) |

### Contrato de saída (`ChunkProcessorOutput`)

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `chunkId` | `string` | Identificador do chunk processado |
| `status` | `string` | `SUCCEEDED` ou `FAILED` |
| `framesCount` | `int` | Quantidade de frames gerados e enviados ao S3 |
| `manifest.bucket` | `string` | Bucket do manifest (somente em SUCCEEDED) |
| `manifest.key` | `string` | Chave do manifest (somente em SUCCEEDED) |
| `error.type` | `string` | Código do erro (somente em FAILED) |
| `error.message` | `string` | Descrição do erro (somente em FAILED) |
| `error.retryable` | `bool` | Se o erro permite retry pela Step Functions (somente em FAILED) |

### Variáveis de ambiente da Lambda

| Variável | Obrigatória | Descrição |
|----------|-------------|-----------|
| `FFMPEG_PATH` | Não | Diretório contendo `ffmpeg` e `ffprobe`. Se ausente, busca em `/opt/bin`, `/opt/ffmpeg` (Lambda Layer) e, por último, baixa automaticamente. |

---

## Regra de Simulação de Erro — `VideoDurationSimulationException`

> **Finalidade:** simular e demonstrar a propagação de exceções por todas as camadas da aplicação durante apresentações e gravações.

### Comportamento

Quando o vídeo processado tiver **exatamente 1303 segundos** de duração total, a aplicação lança uma `VideoDurationSimulationException`. Diferente dos erros operacionais tratados como `ChunkProcessorOutput(FAILED)`, esta exceção **não é capturada pelo Use Case** — ela propaga por todas as camadas até o Lambda runtime, causando falha da invocação.

### Fluxo de propagação

```
VideoFrameExtractor.ExtractFramesAsync
  └─ lança VideoDurationSimulationException (Domain)
       ↑ não capturada pelo ProcessChunkUseCase (apenas FileNotFoundException e InvalidOperationException são tratadas)
ProcessChunkUseCase.ExecuteAsync
  └─ propaga exceção (executa finally de cleanup e relança)
       ↑
FunctionHandler
  └─ captura para logar no CloudWatch e relança
       ↑
Lambda Runtime
  └─ registra a exceção e retorna erro à Step Functions
```

### Como acionar em testes

Forneça um vídeo com exatamente **1303 segundos** (21 minutos e 43 segundos) de duração ou utilize um payload cuja duração real do arquivo seja 1303s. Nomeie o arquivo `sample-1303s.mp4` na raiz do repositório para ativar o teste com `[Fact(Skip)]` automaticamente.

### Log esperado no CloudWatch

```
[SIMULAÇÃO] VideoDurationSimulationException propagada até o Lambda handler.
VideoId=<id> ChunkId=<id> DurationSeconds=1303 Message=[SIMULAÇÃO] A duração do vídeo (1303s) ativou a regra de simulação de erro. Esta exceção é intencional para fins de demonstração.
```

### Código-fonte relevante

| Arquivo | Responsabilidade |
|---------|-----------------|
| `src/Core/VideoProcessor.Domain/Exceptions/VideoDurationSimulationException.cs` | Declaração da exceção de domínio |
| `src/Core/VideoProcessor.Application/Services/VideoFrameExtractor.cs` | Lança a exceção após obter a duração real do vídeo via FFmpeg |
| `src/InterfacesExternas/VideoProcessor.Lambda/Function.cs` | Captura, loga e relança para o Lambda runtime |

### Testes da regra de simulação

| Arquivo | Tipo | Cenários cobertos |
|---------|------|------------------|
| `tests/VideoProcessor.Tests.Unit/Domain/Exceptions/VideoDurationSimulationExceptionTests.cs` | Unit | Propriedades, mensagem, constante `TriggerDurationSeconds`, herança de `Exception` |
| `tests/VideoProcessor.Tests.Unit/Application/UseCases/ProcessChunkUseCaseTests.cs` | Unit | Propagação — `VideoDurationSimulationException` não é capturada pelo UseCase |
| `tests/VideoProcessor.Tests.Unit/Application/Services/VideoFrameExtractorTests.cs` | Unit (Skip) | Lançamento real com vídeo de 1303s — requer `sample-1303s.mp4` na raiz |
| `tests/VideoProcessor.Tests.Bdd/Features/SimulacaoErroDuracao.feature` | BDD | Comportamento ponta a ponta: extrator → UseCase → propagação; constante = 1303 |

---

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
