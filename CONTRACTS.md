# Contrato Input/Output do Chunk Processor

## Versionamento

- **Campo:** `contractVersion` (string)
- **Versões suportadas:** `1.0`

O handler valida `contractVersion` no início. Versões não suportadas resultam em resposta com `status: FAILED` e `error.retryable: false` (sem retry na Step Functions).

## Input (ChunkProcessorInput)

Payload esperado do Map da Step Functions (camelCase):

- `contractVersion` (obrigatório)
- `videoId` (obrigatório)
- `chunk`: `{ chunkId, startSec, endSec }` (obrigatório)
- `source`: `{ bucket, key, etag?, versionId? }` (obrigatório; etag/versionId opcionais)
- `output`: `{ manifestBucket, manifestPrefix, framesBucket?, framesPrefix? }` (obrigatório)
- `executionArn?` (opcional)

## Output (ChunkProcessorOutput)

Resposta (camelCase):

- `chunkId`, `status` (SUCCEEDED | FAILED), `framesCount`
- `manifest?`: `{ bucket, key }` — presente quando `status === "SUCCEEDED"`
- `error?`: `{ type, message, retryable }` — presente quando `status === "FAILED"`
