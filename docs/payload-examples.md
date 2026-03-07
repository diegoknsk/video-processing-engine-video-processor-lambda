# Payloads e Exemplos de Execução — Video Processor

Este documento contém payloads de exemplo para o processador de chunks (Lambda), exemplos de execução do CLI (modo local e modo AWS/S3) e requisitos de ambiente.

---

## 1. Requisitos de Ambiente

### Credenciais AWS
- **CLI local (modo AWS) e invocação Lambda:** credenciais configuradas via `~/.aws/credentials` ou variáveis de ambiente:
  - `AWS_ACCESS_KEY_ID`
  - `AWS_SECRET_ACCESS_KEY`
  - `AWS_DEFAULT_REGION` ou `AWS_REGION`

### Buckets S3
- **Bucket de origem:** com pelo menos um vídeo de teste e permissão `s3:GetObject` para a role/usuário.
- **Bucket de destino:** permissão `s3:PutObject` para gravar frames e manifest.

### Lambda
- **IAM Role:** permissões `s3:GetObject` no bucket de origem e `s3:PutObject` no bucket de destino.
- **Lambda Layer:** FFmpeg publicado em `/opt/bin/ffmpeg` (ou path configurado em `FFMPEG_PATH`).
- **Variável de ambiente:** `FFMPEG_PATH` = `/opt/bin` (ou o path da Layer).

---

## 2. CLI Modo Local (sem regressão)

Processa um vídeo em disco e salva os frames em uma pasta local. Não usa S3.

```bash
dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- \
  --video "C:\Projetos\Fiap\Videos\entrada\teste1.mp4" \
  --interval 2 \
  --output "C:\Projetos\Fiap\Videos\saida" \
  --start 0 \
  --end 10
```

**Resultado esperado:** frames salvos em disco, log de conclusão com total de frames e tempo.

---

## 3. CLI Modo AWS/S3

Baixa o vídeo do S3, extrai frames e faz upload dos frames para o bucket de destino. Usa o mesmo `ProcessChunkUseCase` da Lambda.

**Importante:** `--source-key` deve ser a **key exata** do objeto no S3 (use `aws s3 ls s3://bucket/prefix/ --recursive` para listar e copiar a key). O objeto pode não ter extensão no nome (ex.: `original` em vez de `original.mp4`).

```bash
dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- \
  --mode aws \
  --video-id "video-abc123" \
  --chunk-id "chunk-001" \
  --interval 5 \
  --start 0 \
  --end 60 \
  --source-bucket "video-raw-uploads" \
  --source-key "uploads/video-abc123/original.mp4" \
  --target-bucket "video-processed-frames" \
  --target-prefix "processed/video-abc123/chunk-001/frames/"
```

**Exemplo com buckets dev (video-processing-engine):** ver `docs/testelocalCli.md`.

**Resultado esperado no console:**
```
✓ Processamento concluído!
Status: SUCCEEDED
Frames: 13
Manifest S3: s3://video-processed-frames/processed/video-abc123/chunk-001/manifest.json
```

**Verificar frames no S3:**
```bash
aws s3 ls s3://video-processed-frames/processed/video-abc123/chunk-001/frames/
```

---

## 4. Payload de Input da Lambda (ChunkProcessorInput)

Payload JSON enviado ao handler da Lambda (ex.: pelo Step Functions Map State):

```json
{
  "contractVersion": "1.0",
  "videoId": "video-abc123",
  "chunk": {
    "chunkId": "chunk-001",
    "startSec": 0.0,
    "endSec": 60.0,
    "intervalSec": 5
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
  "executionArn": "arn:aws:states:us-east-1:123456789012:execution:VideoProcessing:exec-001"
}
```

---

## 5. Payload de Output da Lambda — Sucesso (ChunkProcessorOutput)

Resposta da Lambda quando o processamento conclui com sucesso:

```json
{
  "chunkId": "chunk-001",
  "status": "SUCCEEDED",
  "framesCount": 13,
  "manifest": {
    "bucket": "video-processed-frames",
    "key": "processed/video-abc123/chunk-001/manifest.json"
  },
  "error": null
}
```

---

## 6. Payload de Output da Lambda — Falha (ChunkProcessorOutput)

Resposta da Lambda quando o processamento falha (ex.: vídeo não encontrado no S3):

```json
{
  "chunkId": "chunk-001",
  "status": "FAILED",
  "framesCount": 0,
  "manifest": null,
  "error": {
    "type": "VIDEO_NOT_FOUND",
    "message": "Vídeo não encontrado no S3: s3://video-raw-uploads/uploads/video-abc123/original.mp4",
    "retryable": false
  }
}
```

---

## 7. Invocar Lambda via AWS CLI

Salvar o payload em arquivo e invocar a função:

```bash
# Salvar payload (ajuste bucket/key conforme seu ambiente)
cat > test-payload.json << 'EOF'
{
  "contractVersion": "1.0",
  "videoId": "video-abc123",
  "chunk": {
    "chunkId": "chunk-001",
    "startSec": 0.0,
    "endSec": 60.0,
    "intervalSec": 5
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
  }
}
EOF

# Invocar Lambda
aws lambda invoke \
  --function-name VideoProcessor \
  --payload file://test-payload.json \
  --cli-binary-format raw-in-base64-out \
  response.json

cat response.json
```

---

## 8. Verificar frames no S3 após invocação

```bash
aws s3 ls s3://video-processed-frames/processed/video-abc123/chunk-001/frames/
```

Os frames são salvos com o padrão `frame_NNNN_Xs.jpg` (ex.: `frame_0001_0s.jpg`, `frame_0002_5s.jpg`).

---

## 9. Notas de configuração da Lambda

| Configuração           | Recomendação                          |
|------------------------|----------------------------------------|
| **Timeout**            | ≥ 300 s (5 min) para chunks de até 60 s |
| **Memória**            | ≥ 2048 MB                             |
| **Ephemeral storage**  | ≥ 1024 MB                             |
| **Variável de ambiente** | `FFMPEG_PATH` = `/opt/bin`          |
| **Layer**              | FFmpeg binário estático para Amazon Linux 2023 em `/opt/bin/ffmpeg` |
