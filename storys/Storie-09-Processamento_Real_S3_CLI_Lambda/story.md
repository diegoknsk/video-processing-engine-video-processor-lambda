# Storie-09: Processamento Real com S3 — CLI Local, CLI AWS e Lambda para Step Functions

## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 07/03/2026

## Descrição
Como engenheiro do Video Processing Engine, quero que a lógica de processamento de vídeo (extração de frames) seja real e reutilizável entre o CLI local, um CLI em modo AWS/S3 e a Lambda existente, para que a Lambda deixe de ser mockada, passe a executar o processamento real e fique pronta para ser invocada pelo Map State de um Step Functions, enquanto o CLI local continue funcionando sem nenhuma regressão.

## Objetivo
Extrair e consolidar a orquestração de processamento em um `ProcessChunkUseCase` na camada Application, criar o port `IS3VideoStorage` no Domain com implementação no Infra, adicionar modo AWS/S3 ao CLI existente, e adaptar o `Function.cs` da Lambda para receber um `ChunkProcessorInput` real e retornar um `ChunkProcessorOutput` — eliminando o Hello World mockado e entregando um processador de chunk stateless pronto para uso no Step Functions.

## Contexto Arquitetural

### Fluxo atual (local)
```
CLI → VideoFrameExtractor → frames em disco local
Lambda → Hello World mockado (sem processamento real)
```

### Fluxo alvo desta story
```
CLI modo local: VideoPath local → VideoFrameExtractor → frames em disco local    (sem regressão)
CLI modo AWS:   S3 sourceBucket/sourceKey → /tmp → VideoFrameExtractor → frames em /tmp → S3 targetBucket/targetPrefix
Lambda:         ChunkProcessorInput (S3) → ProcessChunkUseCase → FrameExtractionResult → Upload S3 → ChunkProcessorOutput
```

### Reaproveitamento da lógica (ponto-chave de arquitetura)
O `ProcessChunkUseCase` (Application) é a unidade central reaproveitada:
- CLI modo AWS o chama diretamente após parsear argumentos em um `ChunkProcessorInput`.
- Lambda o chama após desserializar o evento JSON em um `ChunkProcessorInput`.
- CLI modo local **não** usa o use case — continua usando `VideoFrameExtractor` diretamente como hoje.
- Nenhuma lógica de negócio fica duplicada entre CLI AWS e Lambda.

### Contrato do processador (stateless)
**Input** — `ChunkProcessorInput` (já existe em Domain):
```json
{
  "contractVersion": "1.0",
  "videoId": "video-abc123",
  "chunk": {
    "chunkId": "chunk-001",
    "startSec": 0.0,
    "endSec": 60.0
  },
  "source": {
    "bucket": "video-raw-uploads",
    "key": "uploads/video-abc123/original.mp4"
  },
  "output": {
    "manifestBucket": "video-processed-frames",
    "manifestPrefix": "processed/video-abc123/chunk-001/",
    "framesBucket": "video-processed-frames",
    "framesPrefix": "processed/video-abc123/chunk-001/frames/"
  },
  "executionArn": "arn:aws:states:us-east-1:123456789012:execution:VideoProcessingStateMachine:exec-001"
}
```

**Output** — `ChunkProcessorOutput` (já existe em Domain):
```json
{
  "chunkId": "chunk-001",
  "status": "SUCCEEDED",
  "framesCount": 31,
  "manifest": {
    "bucket": "video-processed-frames",
    "key": "processed/video-abc123/chunk-001/manifest.json"
  },
  "error": null
}
```

### O que fica fora de escopo nesta story
- **DynamoDB** — nenhum update de status ou escrita em tabela.
- **Update de status** no Step Functions ou em qualquer datastore.
- **Geração e upload de manifest.json detalhado** — o manifest retornado no output é só a referência de localização; o arquivo real em S3 é opcional/mínimo nesta story.
- **Finalização ZIP** dos frames.
- **Tratamento avançado de falhas no Step Functions** (Retry, Catch, TaskTimeout).
- **Distributed Map** — a story prepara o Lambda para uso no Map State, mas não implementa a state machine.
- **Idempotência** e detecção de reprocessamento.
- **Mensageria** (SQS/SNS) — sem integração real.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, AWS Lambda, Amazon S3, Xabe.FFmpeg
- **Arquivos criados/modificados:**
  - `src/Core/VideoProcessor.Domain/Ports/IS3VideoStorage.cs` (**novo** — port para S3)
  - `src/Infra/VideoProcessor.Infra/S3/S3VideoStorage.cs` (**novo** — implementação S3)
  - `src/Infra/VideoProcessor.Infra/VideoProcessor.Infra.csproj` (adicionar AWSSDK.S3)
  - `src/Core/VideoProcessor.Application/UseCases/ProcessChunkUseCase.cs` (**novo** — orquestra extração + upload)
  - `src/Core/VideoProcessor.Application/VideoProcessor.Application.csproj` (sem mudança ou mínima)
  - `src/InterfacesExternas/VideoProcessor.Lambda/Function.cs` (**modificar** — substituir mock pelo use case real)
  - `src/InterfacesExternas/VideoProcessor.Lambda/VideoProcessor.Lambda.csproj` (verificar AWSSDK.S3 se necessário)
  - `src/InterfacesExternas/VideoProcessor.CLI/Program.cs` (**modificar** — adicionar modo AWS, preservar modo local)
  - `tests/VideoProcessor.Tests.Unit/Application/UseCases/ProcessChunkUseCaseTests.cs` (**novo**)
  - `tests/VideoProcessor.Tests.Unit/Infra/S3/S3VideoStorageTests.cs` (**novo**, mocks de S3)
  - `docs/payload-examples.md` (**novo** — payloads de exemplo e comandos de execução)
- **Componentes criados/modificados:**
  - `IS3VideoStorage` (port) — DownloadToTempAsync, UploadFramesAsync
  - `S3VideoStorage` (implementação Infra) — AmazonS3Client
  - `ProcessChunkUseCase` (Application) — orquestrador stateless
  - `Function.cs` (Lambda) — recebe ChunkProcessorInput, chama use case, retorna ChunkProcessorOutput
  - `Program.cs` (CLI) — modo `--mode aws` para teste local com S3 real
- **Pacotes/Dependências:**
  - `AWSSDK.S3` (3.7.x — última estável) — adicionado ao `VideoProcessor.Infra.csproj`
  - `Xabe.FFmpeg` (já existente em Application) — sem mudança de versão
  - `Amazon.Lambda.Core` (2.8.0 — já existente no Lambda)
  - `Microsoft.Extensions.DependencyInjection` (10.0.0 — já existente no Lambda)

## Dependências e Riscos (para estimativa)

### Dependências obrigatórias
- **Storie-05** concluída — `IVideoFrameExtractor` e `VideoFrameExtractor` funcionando localmente.
- **Storie-05.1** (parcialmente) — parâmetros `startSec`/`endSec` devem estar disponíveis no extractor (já implementados na interface e na classe).
- **Storie-08** concluída — estrutura de diretórios `src/Core`, `src/Infra`, `src/InterfacesExternas` em vigor.

### Pré-condições de ambiente
- Bucket S3 de origem com pelo menos um vídeo de teste acessível.
- Bucket S3 de destino com permissão `s3:PutObject`.
- AWS credentials configuradas localmente (via `~/.aws/credentials` ou variáveis de ambiente) para execução do CLI modo AWS.
- Lambda com IAM role com permissões `s3:GetObject` no bucket de origem e `s3:PutObject` no bucket de destino.

### Riscos
- FFmpeg no Lambda: binário deve estar acessível via Lambda Layer; caminho `/opt/bin/ffmpeg` deve ser configurado no `FFmpegSetup` ou em um `FFmpegConfigurator` novo. **Mitigar:** incluir lógica de detecção de path na subtask de adaptação da Lambda.
- `/tmp` limitado (512 MB padrão, até 10 GB configurável): chunks grandes podem exceder limite. **Mitigar:** documentar configuração de `/tmp` ephemeral storage e limitar vídeos de teste ao tamanho adequado.
- Timeout Lambda: processamento de chunks longos pode exceder timeout configurado. **Mitigar:** documentar configuração de timeout recomendada (≥ 300s) nas notas da subtask da Lambda.
- AWSSDK.S3 adiciona ~10 MB ao pacote Lambda: **Mitigar:** aceitar — já previsto na arquitetura.

## Subtasks
- [x] [Subtask 01: Criar port IS3VideoStorage no Domain](./subtask/Subtask-01-Criar_Port_IS3VideoStorage.md)
- [x] [Subtask 02: Implementar S3VideoStorage no Infra](./subtask/Subtask-02-Implementar_S3VideoStorage_Infra.md)
- [x] [Subtask 03: Criar ProcessChunkUseCase na Application](./subtask/Subtask-03-Criar_ProcessChunkUseCase.md)
- [x] [Subtask 04: Adaptar Lambda Function.cs para processamento real](./subtask/Subtask-04-Adaptar_Lambda_Function.md)
- [x] [Subtask 05: Preservar CLI local e adicionar modo AWS/S3](./subtask/Subtask-05-CLI_Modo_AWS_S3.md)
- [x] [Subtask 06: Testes unitários — UseCase e S3VideoStorage](./subtask/Subtask-06-Testes_Unitarios.md)
- [x] [Subtask 07: Testes manuais locais e documentação de payloads](./subtask/Subtask-07-Testes_Manuais_Documentacao.md)

## Critérios de Aceite da História

### Funcionais
- [ ] CLI modo local continua funcionando sem regressão: `dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --video <path> --interval 2 --output <path> --start 0 --end 10` extrai frames para disco como hoje.
- [ ] CLI modo AWS executa com sucesso: `dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --mode aws --video-id <id> --interval <n> --start <s> --end <e> --source-bucket <b> --source-key <k> --target-bucket <b> --target-prefix <p>` — baixa vídeo do S3, extrai frames, faz upload dos frames para S3 e imprime o `ChunkProcessorOutput` no console.
- [ ] Lambda recebe `ChunkProcessorInput` serializado em JSON, executa o processamento real (download do S3 → extração de frames via FFmpeg → upload dos frames para S3) e retorna `ChunkProcessorOutput` com `status = "SUCCEEDED"` e `framesCount > 0`.
- [ ] Em caso de falha (ex.: vídeo não encontrado no S3), Lambda retorna `ChunkProcessorOutput` com `status = "FAILED"` e `error` preenchido, sem lançar exceção não tratada.
- [ ] Frames são salvos no bucket S3 de destino sob o prefixo `{output.framesPrefix}frame_NNNN_Xs.jpg`.

### Técnicos / Arquiteturais
- [ ] `ProcessChunkUseCase` reside em Application e não referencia nenhuma biblioteca específica de AWS ou de CLI — apenas `IVideoFrameExtractor` e `IS3VideoStorage` via injeção.
- [ ] `IS3VideoStorage` reside em Domain (Port), sem dependência de AWSSDK — apenas contratos de I/O.
- [ ] `S3VideoStorage` reside em Infra e é a única classe que depende de `AWSSDK.S3`.
- [ ] CLI modo local não passa por `ProcessChunkUseCase` — preserva o caminho direto via `VideoFrameExtractor`.
- [ ] Lambda usa DI (construtor ou `ServiceCollection`) para resolver `ProcessChunkUseCase`, `IS3VideoStorage` e `IVideoFrameExtractor`.
- [ ] Nenhuma lógica de processamento duplicada entre CLI modo AWS e Lambda — ambos chamam `ProcessChunkUseCase`.
- [ ] `dotnet build` da solução completa passa sem erros após as mudanças.
- [ ] `dotnet test` executa todos os testes existentes sem regressão; novos testes da story passam.

### Testes
- [ ] Testes unitários cobrem: `ProcessChunkUseCase` (sucesso, falha no download, falha na extração, falha no upload) com mocks de `IS3VideoStorage` e `IVideoFrameExtractor`.
- [ ] Testes unitários cobrem: `S3VideoStorage` com mock do `AmazonS3Client` (ou wrapper testável).
- [ ] Cobertura de testes nos novos arquivos ≥ 80%.

### Documentação
- [ ] `docs/payload-examples.md` criado com: payload de input/output de exemplo, exemplos de execução CLI local, CLI modo AWS e invocação Lambda via `aws lambda invoke`.

## Rastreamento (dev tracking)
- **Início:** 07/03/2026, às 15:50 (Brasília)
- **Fim:** 07/03/2026, às 17:05 (Brasília)
- **Tempo total de desenvolvimento:** 1h 15min
