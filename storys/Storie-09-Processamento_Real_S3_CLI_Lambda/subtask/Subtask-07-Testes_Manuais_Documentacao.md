# Subtask 07: Testes manuais locais e documentação de payloads

## Descrição
Criar `docs/payload-examples.md` com payloads de exemplo completos (input/output), exemplos de execução CLI local, CLI modo AWS e invocação Lambda, além de checklist de configuração de ambiente. Executar os testes manuais de ponta a ponta (CLI local, CLI modo AWS, Lambda) e registrar os resultados. Esta subtask fecha a story com validação e documentação executável.

## Passos de implementação

### 1. Criar `docs/payload-examples.md`
O documento deve conter as seguintes seções:

#### Seção 1 — Requisitos de Ambiente
- AWS credentials configuradas (`~/.aws/credentials` ou variáveis de ambiente).
- Variáveis necessárias: `AWS_REGION`, `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` (ou profile).
- Bucket S3 de origem com um vídeo de teste carregado.
- Bucket S3 de destino com permissão `PutObject`.
- Lambda com IAM role com `s3:GetObject` (origem) e `s3:PutObject` (destino).
- Lambda com Layer de FFmpeg configurada.

#### Seção 2 — CLI Modo Local (sem regressão)
```bash
dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- \
  --video "C:\Projetos\Fiap\Videos\entrada\teste1.mp4" \
  --interval 2 \
  --output "C:\Projetos\Fiap\Videos\saida" \
  --start 0 \
  --end 10
```
Resultado esperado: frames salvos em disco, log de conclusão com total de frames e tempo.

#### Seção 3 — CLI Modo AWS/S3
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
Resultado esperado no console:
```
✓ Processamento concluído!
Status: SUCCEEDED
Frames: 13
S3 destino: s3://video-processed-frames/processed/video-abc123/chunk-001/frames/
```
Confirmar frames visíveis via: `aws s3 ls s3://video-processed-frames/processed/video-abc123/chunk-001/frames/`

#### Seção 4 — Payload de Input da Lambda (ChunkProcessorInput)
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

#### Seção 5 — Payload de Output da Lambda (ChunkProcessorOutput — sucesso)
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

#### Seção 6 — Payload de Output da Lambda (ChunkProcessorOutput — falha)
```json
{
  "chunkId": "chunk-001",
  "status": "FAILED",
  "framesCount": 0,
  "manifest": null,
  "error": {
    "code": "VIDEO_NOT_FOUND",
    "message": "Vídeo não encontrado no S3: s3://video-raw-uploads/uploads/video-abc123/original.mp4"
  }
}
```

#### Seção 7 — Invocar Lambda via CLI AWS
```bash
# Salvar payload em arquivo local
cat > /tmp/test-payload.json << 'EOF'
{
  "contractVersion": "1.0",
  "videoId": "video-abc123",
  ...
}
EOF

# Invocar Lambda
aws lambda invoke \
  --function-name VideoProcessor \
  --payload file:///tmp/test-payload.json \
  --cli-binary-format raw-in-base64-out \
  /tmp/response.json

cat /tmp/response.json
```

#### Seção 8 — Verificar frames no S3 após invocação
```bash
aws s3 ls s3://video-processed-frames/processed/video-abc123/chunk-001/frames/
```

#### Seção 9 — Notas de configuração da Lambda
- Timeout: ≥ 300s.
- Memória: ≥ 2048 MB.
- Ephemeral storage: ≥ 1024 MB.
- Variável de ambiente `FFMPEG_PATH`: `/opt/bin`.
- Layer de FFmpeg: publicar binário estático para Amazon Linux 2023 em `/opt/bin/ffmpeg`.

### 2. Executar testes manuais e registrar resultado

Executar na ordem:
1. `dotnet test` — confirmar todos os testes passando.
2. CLI local com vídeo existente — confirmar sem regressão.
3. CLI modo AWS com bucket e vídeo reais — confirmar frames no S3.
4. Lambda via `aws lambda invoke` — confirmar output com `status: SUCCEEDED` e frames no S3.

## Formas de teste
- Validar que `docs/payload-examples.md` existe e contém os 9 blocos listados.
- Executar cada comando da documentação e confirmar que produz o resultado descrito.
- Confirmar via `aws s3 ls` que os frames aparecem no bucket de destino após cada execução.

## Critérios de aceite da subtask
- [ ] `docs/payload-examples.md` criado com payloads de input/output completos, exemplos de CLI local, CLI AWS e invocação Lambda via `aws lambda invoke`.
- [ ] Seção de requisitos de ambiente (credenciais, buckets, Layer FFmpeg, IAM role) presente e clara.
- [ ] Teste manual CLI local executado com sucesso (sem regressão confirmada).
- [ ] Teste manual CLI modo AWS executado com frames visíveis no bucket S3 de destino.
- [ ] Teste manual Lambda executado com output `status: SUCCEEDED` e frames visíveis no bucket S3 de destino.
- [ ] `dotnet test` passa após todas as mudanças da story (zero falhas, zero regressões).
