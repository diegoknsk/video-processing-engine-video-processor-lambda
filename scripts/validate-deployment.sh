#!/usr/bin/env bash
# Script de validação end-to-end pós-deploy do Lambda.
# Executa Step Functions com payload de teste, verifica S3 e CloudWatch.
# Uso: bash scripts/validate-deployment.sh --state-machine-arn <ARN> --execution-name <nome> --payload-file <path> --output-bucket <bucket> --output-prefix <prefix>

set -e

STATE_MACHINE_ARN=""
EXECUTION_NAME=""
PAYLOAD_FILE=""
OUTPUT_BUCKET=""
OUTPUT_PREFIX=""
TIMEOUT_SEC=120
POLL_INTERVAL=5

usage() {
  echo "Uso: $0 --state-machine-arn <ARN> --execution-name <nome> --payload-file <path> --output-bucket <bucket> --output-prefix <prefix>"
  echo ""
  echo "Argumentos obrigatórios:"
  echo "  --state-machine-arn   ARN da Step Functions"
  echo "  --execution-name      Nome da execução (ex: test-deploy-\$(date +%s))"
  echo "  --payload-file        Caminho para JSON de payload (ex: test-payloads/smoke-payload.json)"
  echo "  --output-bucket       Bucket onde esperar manifest/done"
  echo "  --output-prefix       Prefixo no bucket (ex: manifests/test-video-123/chunk-0/)"
  exit 1
}

while [[ $# -gt 0 ]]; do
  case $1 in
    --state-machine-arn)
      STATE_MACHINE_ARN="$2"
      shift 2
      ;;
    --execution-name)
      EXECUTION_NAME="$2"
      shift 2
      ;;
    --payload-file)
      PAYLOAD_FILE="$2"
      shift 2
      ;;
    --output-bucket)
      OUTPUT_BUCKET="$2"
      shift 2
      ;;
    --output-prefix)
      OUTPUT_PREFIX="$2"
      shift 2
      ;;
    *)
      echo "Argumento desconhecido: $1"
      usage
      ;;
  esac
done

if [[ -z "$STATE_MACHINE_ARN" || -z "$EXECUTION_NAME" || -z "$PAYLOAD_FILE" || -z "$OUTPUT_BUCKET" || -z "$OUTPUT_PREFIX" ]]; then
  echo "Erro: todos os argumentos obrigatórios devem ser fornecidos."
  usage
fi

if [[ ! -f "$PAYLOAD_FILE" ]]; then
  echo "Erro: arquivo de payload não encontrado: $PAYLOAD_FILE"
  exit 1
fi

echo "=== Iniciando execução Step Functions ==="
EXEC_ARN=$(aws stepfunctions start-execution \
  --state-machine-arn "$STATE_MACHINE_ARN" \
  --name "$EXECUTION_NAME" \
  --input "file://$PAYLOAD_FILE" \
  --query 'executionArn' --output text)

echo "Execution ARN: $EXEC_ARN"

echo "=== Aguardando conclusão (timeout ${TIMEOUT_SEC}s) ==="
ELAPSED=0
while true; do
  STATUS=$(aws stepfunctions describe-execution --execution-arn "$EXEC_ARN" --query 'status' --output text)
  echo "  Status: $STATUS (elapsed: ${ELAPSED}s)"

  if [[ "$STATUS" == "SUCCEEDED" ]]; then
    break
  fi
  if [[ "$STATUS" == "FAILED" || "$STATUS" == "TIMED_OUT" || "$STATUS" == "ABORTED" ]]; then
    echo "Erro: execução falhou com status $STATUS"
    exit 1
  fi

  sleep $POLL_INTERVAL
  ELAPSED=$((ELAPSED + POLL_INTERVAL))
  if [[ $ELAPSED -ge $TIMEOUT_SEC ]]; then
    echo "Erro: timeout após ${TIMEOUT_SEC}s"
    exit 1
  fi
done

echo "=== Verificando artefatos no S3 ==="
PREFIX="${OUTPUT_PREFIX%/}"
[[ -n "$PREFIX" ]] && PREFIX="${PREFIX}/"
MANIFEST_PATH="s3://${OUTPUT_BUCKET}/${PREFIX}manifest.json"
DONE_PATH="s3://${OUTPUT_BUCKET}/${PREFIX}done.json"

if ! aws s3 ls "$MANIFEST_PATH" > /dev/null 2>&1; then
  echo "Erro: manifest.json não encontrado em $MANIFEST_PATH"
  exit 1
fi
echo "  manifest.json: OK"

if ! aws s3 ls "$DONE_PATH" > /dev/null 2>&1; then
  echo "Erro: done.json não encontrado em $DONE_PATH"
  exit 1
fi
echo "  done.json: OK"

if command -v jq &> /dev/null; then
  TMP_MANIFEST=$(mktemp)
  aws s3 cp "$MANIFEST_PATH" "$TMP_MANIFEST" --quiet
  if ! jq empty "$TMP_MANIFEST" 2>/dev/null; then
    echo "Aviso: manifest.json não é JSON válido"
  else
    echo "  manifest.json: JSON válido"
  fi
  rm -f "$TMP_MANIFEST"
fi

echo "=== Consultando logs do Lambda (últimos 5 minutos) ==="
if aws logs tail /aws/lambda/video-processor-chunk-worker --since 5m 2>/dev/null | grep -qE "videoId|chunkId|test-video-123|chunk-0"; then
  echo "  Logs contêm videoId/chunkId esperados: OK"
else
  echo "  Aviso: videoId ou chunkId não encontrados nos logs (pode ser normal se execução recente)"
fi

echo ""
echo "=== Validação concluída com sucesso ==="
exit 0
