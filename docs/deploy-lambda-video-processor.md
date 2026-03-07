# Deploy Lambda — Video Processor (Chunk Worker)

Documentação do pipeline de deploy da Lambda de processamento de chunks (Story 09) via GitHub Actions: estratégia de branches, variáveis e configuração FFmpeg.

## Estratégia de branches

| Branch | Comportamento                    |
|--------|----------------------------------|
| **main** | Deploy automático ao push/merge  |
| **dev**  | Deploy automático ao push       |

Tanto **main** quanto **dev** fazem deploy na **mesma** função Lambda (configurada pela variável `LAMBDA_FUNCTION_NAME`). Push em outras branches (ex.: `feature/xyz`) executa apenas **build e testes**; o job de deploy não roda.

O job `deploy` só é executado quando o push é em `main` ou `dev`.

## Variáveis do repositório

Configure em **Settings → Secrets and variables → Actions → Variables**:

| Variável                | Obrigatória | Descrição                    | Valor padrão (se vazio)     |
|-------------------------|-------------|------------------------------|-----------------------------|
| `LAMBDA_FUNCTION_NAME`  | Não         | Nome da função Lambda na AWS | `video-processor-chunk-worker` |

Se não definir a variável, o workflow usa o nome padrão acima. A função deve existir na AWS antes do primeiro deploy.

## Secrets necessários

| Secret                  | Descrição |
|-------------------------|-----------|
| `AWS_ACCESS_KEY_ID`     | Access Key da conta AWS |
| `AWS_SECRET_ACCESS_KEY` | Secret Key da conta AWS |
| `AWS_SESSION_TOKEN`     | Token de sessão (obrigatório para credenciais temporárias, ex.: AWS Academy) |
| `AWS_REGION`            | Região do Lambda (ex.: `us-east-1`) |

## Path do projeto e handler

- **Projeto Lambda:** `src/InterfacesExternas/VideoProcessor.Lambda`
- **Handler:** `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler`

O workflow gera o ZIP com `dotnet lambda package -pl src/InterfacesExternas/VideoProcessor.Lambda`, incluindo referências a Application, Domain e Infra.

## Layer FFmpeg e variáveis da Lambda

A Lambda usa **FFmpeg** (via Xabe.FFmpeg) para extração de frames. O binário **não** é empacotado no ZIP pelo workflow; a função na AWS deve ter FFmpeg disponível via **Lambda Layer** ou variável de ambiente.

### Onde a Lambda procura o FFmpeg

A aplicação procura, nesta ordem:

1. **Variável de ambiente `FFMPEG_PATH`** — diretório onde estão `ffmpeg` e `ffprobe`.
2. **`/opt/bin`** — caminho típico de um layer que expõe binários em `bin`.
3. **`/opt/ffmpeg`** — caminho típico de layers que expõem FFmpeg em `ffmpeg`.

Recomendação: usar um **Lambda Layer** com FFmpeg para Amazon Linux 2 e anexar à função. O layer costuma expor os binários em `/opt/ffmpeg` ou `/opt/bin`.

**Sistema de arquivos no Lambda:** No ambiente AWS Lambda, apenas o diretório **`/tmp`** é gravável; `/var/task` (código da função) é somente leitura. Se o FFmpeg não for encontrado via Layer ou `FFMPEG_PATH`, a aplicação usa **`/tmp/.ffmpeg`** como fallback para download dos binários (consumindo tempo e espaço de `/tmp`, limitado a 512 MB). Por isso, configurar um **Lambda Layer** com FFmpeg em `/opt/ffmpeg` ou `/opt/bin` é a abordagem recomendada.

### Como anexar o Layer à função

**Console AWS:**

1. Lambda → Functions → selecione a função (prod ou dev).
2. Configuration → Layers → Add layer.
3. Escolha “Custom layers” e selecione o layer que contém FFmpeg (ou crie um com FFmpeg para Amazon Linux 2).
4. Save.

**AWS CLI:**

```bash
# Substitua FUNCTION_NAME e LAYER_ARN
aws lambda update-function-configuration \
  --function-name video-processor-chunk-worker \
  --layers arn:aws:lambda:us-east-1:ACCOUNT_ID:layer:ffmpeg-layer:1
```

O **workflow de deploy não altera** a configuração de layers; ele só atualiza código e handler. Layers configurados na função permanecem após cada deploy.

### Variáveis de ambiente úteis na Lambda

| Variável       | Descrição |
|----------------|-----------|
| `FFMPEG_PATH`  | (Opcional) Diretório dos binários `ffmpeg` e `ffprobe`. Use se o layer expõe em um path diferente de `/opt/bin` ou `/opt/ffmpeg`. |

Outras variáveis (buckets, etc.) dependem da configuração da aplicação e devem ser definidas na função (Console ou IaC).

## Checklist pós-deploy

Após cada deploy (main ou dev), valide:

- [ ] **Pipeline** — O workflow do GitHub Actions concluiu com sucesso (build, test, deploy).
- [ ] **Função atualizada** — No console AWS (Lambda → Function), “Last modified” corresponde ao horário do deploy.
- [ ] **Handler** — Configuration → Runtime settings: handler = `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler`.
- [ ] **Invocação de teste** — Invocar a função com payload `ChunkProcessorInput` válido retorna HTTP 200 e corpo coerente com `ChunkProcessorOutput` (ou documentar que a validação E2E é feita em outro momento).
- [ ] **Logs** — CloudWatch Logs do grupo da função mostram logs do processamento (videoId, chunkId, etc.), sem erro de “FFmpeg não encontrado” se o layer estiver configurado.

Exemplos de payload e invocação: [payload-examples.md](payload-examples.md) e [INVOCATION_GUIDE.md](INVOCATION_GUIDE.md).

## Execução manual do workflow

Em **Actions → Deploy Lambda → Run workflow** você pode disparar o pipeline manualmente:

- Selecione a branch (ex.: `main` ou `dev`).
- O deploy atualizará a mesma função Lambda (`LAMBDA_FUNCTION_NAME`).

## Validação do pipeline

O pipeline (build → test → package → deploy) deve ser executado com sucesso em ambas as branches:

- **Push em `dev` ou `main`:** build-and-test roda; deploy atualiza a função Lambda configurada em `LAMBDA_FUNCTION_NAME`.

Nenhuma regressão em relação à Story 09: o código implantado continua sendo o Lambda com ProcessChunkUseCase, S3 e FFmpeg; o workflow não reverte para um Lambda “Hello World”.

## Referências

- Handler e payload: [INVOCATION_GUIDE.md](INVOCATION_GUIDE.md), [payload-examples.md](payload-examples.md).
- Workflow: `.github/workflows/deploy-lambda.yml`.
