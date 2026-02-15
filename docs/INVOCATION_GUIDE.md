# Guia de Invocação do Lambda — Video Processor

Este documento descreve como invocar manualmente o Lambda `video-processor-chunk-worker` para testes e validação.

## Pré-requisitos

- AWS CLI instalado e configurado
- Credenciais AWS válidas (Access Key, Secret Key, Session Token para AWS Academy)
- Acesso ao Console AWS
- Função Lambda já criada na AWS

## 1. Invocação via Console AWS

### Passo 1: Acessar a Função Lambda

1. Faça login no [AWS Console](https://console.aws.amazon.com/)
2. Navegue para **Lambda** → **Functions**
3. Selecione a função `video-processor-chunk-worker`

### Passo 2: Criar Test Event

1. Clique na aba **Test**
2. Clique em **Create new event**
3. Configure o evento:
   - **Event name:** `HelloWorldTest`
   - **Event JSON:** copie o conteúdo de `tests/payloads/hello-world-test.json`

```json
{
  "message": "Test invocation from Console",
  "testId": "test-001"
}
```

4. Clique em **Save**

### Passo 3: Executar Teste

1. Selecione o evento `HelloWorldTest`
2. Clique em **Test**
3. Aguarde a execução (geralmente < 1 segundo)

### Passo 4: Validar Resposta

A resposta deve conter:

```json
{
  "message": "Hello World from Video Processor Lambda",
  "version": "1.0.0",
  "timestamp": "2026-02-15T17:14:00.0000000Z",
  "environment": "dev"
}
```

Você também verá:
- **Status:** Succeeded
- **Duration:** ~50-100ms
- **Memory used:** ~50-100 MB
- **Request ID:** (UUID único)

### Screenshots

Veja exemplos visuais em `docs/screenshots/console-invocation.png`.

---

## 2. Invocação via AWS CLI

### Passo 1: Validar Instalação e Credenciais

Verifique se o AWS CLI está instalado:

```bash
aws --version
```

Verifique suas credenciais:

```bash
aws sts get-caller-identity
```

Se as credenciais estiverem corretas, você verá seu UserId, Account e Arn.

### Passo 2: Invocar Lambda

**Comando básico:**

```bash
aws lambda invoke \
  --function-name video-processor-chunk-worker \
  --payload '{}' \
  --cli-binary-format raw-in-base64-out \
  response.json
```

**Comando com região específica:**

```bash
aws lambda invoke \
  --function-name video-processor-chunk-worker \
  --payload '{}' \
  --cli-binary-format raw-in-base64-out \
  --region us-east-1 \
  response.json
```

**Comando com payload de arquivo:**

```bash
aws lambda invoke \
  --function-name video-processor-chunk-worker \
  --payload file://tests/payloads/hello-world-test.json \
  --cli-binary-format raw-in-base64-out \
  response.json
```

### Passo 3: Verificar Resposta

Após executar o comando, verifique o arquivo de resposta:

**Linux/Mac:**
```bash
cat response.json
```

**Windows PowerShell:**
```powershell
Get-Content response.json
```

A resposta deve conter o payload Hello World esperado.

### Flags Importantes

| Flag | Descrição |
|------|-----------|
| `--function-name` | Nome da função Lambda (obrigatório) |
| `--payload` | Payload de entrada (JSON string ou `file://path`) |
| `--cli-binary-format raw-in-base64-out` | Necessário para AWS CLI v2 (evita erro de base64) |
| `--region` | Região AWS (ex: `us-east-1`). Se omitido, usa região default do perfil |
| `--invocation-type` | `RequestResponse` (default), `Event` (async), ou `DryRun` |

---

## 3. Verificar Logs no CloudWatch

### Passo 1: Acessar CloudWatch Logs

1. No AWS Console, navegue para **CloudWatch** → **Logs** → **Log groups**
2. Procure pelo log group: `/aws/lambda/video-processor-chunk-worker`
3. Clique no log group

### Passo 2: Localizar Log Stream

1. Os log streams são organizados por data e hora
2. Clique no stream mais recente (topo da lista)

### Passo 3: Validar Conteúdo do Log

Você deve ver:

```
START RequestId: 12345678-1234-1234-1234-123456789012 Version: $LATEST

Hello World invoked at 2026-02-15T17:14:00.0000000Z

END RequestId: 12345678-1234-1234-1234-123456789012

REPORT RequestId: 12345678-1234-1234-1234-123456789012
Duration: 52.34 ms
Billed Duration: 53 ms
Memory Size: 512 MB
Max Memory Used: 65 MB
Init Duration: 120.45 ms
```

### Passo 4: Tail Logs via CLI (Tempo Real)

Para ver logs em tempo real:

```bash
aws logs tail /aws/lambda/video-processor-chunk-worker --follow
```

Para ver logs das últimas 10 invocações:

```bash
aws logs tail /aws/lambda/video-processor-chunk-worker --since 10m
```

### Delay de Logs

**Importante:** Logs podem levar 5-10 segundos para aparecer no CloudWatch após a invocação. Aguarde alguns segundos antes de verificar.

---

## 4. Troubleshooting Comum

### Erro: "The security token included in the request is expired"

**Causa:** Credenciais AWS expiradas (comum no AWS Academy).

**Solução:**
1. Acesse o AWS Academy e gere novas credenciais temporárias
2. Atualize as variáveis de ambiente ou arquivo `~/.aws/credentials`:
   ```
   [default]
   aws_access_key_id = YOUR_ACCESS_KEY
   aws_secret_access_key = YOUR_SECRET_KEY
   aws_session_token = YOUR_SESSION_TOKEN
   region = us-east-1
   ```

### Erro: "Function not found: arn:aws:lambda:..."

**Causa:** Região AWS incorreta ou nome da função errado.

**Solução:**
1. Verifique a região: `aws configure get region`
2. Liste suas funções Lambda: `aws lambda list-functions`
3. Confirme o nome exato da função

### Erro: "Invalid base64" (AWS CLI)

**Causa:** AWS CLI v2 requer flag `--cli-binary-format`.

**Solução:** Adicione a flag:
```bash
--cli-binary-format raw-in-base64-out
```

### Logs não aparecem no CloudWatch

**Causa:** Delay de propagação ou função não executou.

**Solução:**
1. Aguarde 10-15 segundos e recarregue a página
2. Verifique se a função foi invocada com sucesso (resposta status 200)
3. Verifique se o log group `/aws/lambda/video-processor-chunk-worker` existe
4. Verifique permissões IAM da função (role deve ter `logs:CreateLogGroup`, `logs:CreateLogStream`, `logs:PutLogEvents`)

### Erro: "Access Denied" ao invocar Lambda

**Causa:** IAM role ou credenciais sem permissão `lambda:InvokeFunction`.

**Solução:**
1. Verifique suas credenciais: `aws sts get-caller-identity`
2. Verifique políticas IAM do seu usuário/role
3. Adicione política `AWSLambdaFullAccess` ou permissão específica:
   ```json
   {
     "Effect": "Allow",
     "Action": "lambda:InvokeFunction",
     "Resource": "arn:aws:lambda:*:*:function:video-processor-chunk-worker"
   }
   ```

---

## 5. Referências Rápidas

### Payload de Teste

Use o arquivo `tests/payloads/hello-world-test.json` para testes no Console AWS.

### Resposta Esperada

```json
{
  "message": "Hello World from Video Processor Lambda",
  "version": "1.0.0",
  "timestamp": "2026-02-15T17:14:00.0000000Z",
  "environment": "dev"
}
```

### Comandos Úteis

**Invocar Lambda:**
```bash
aws lambda invoke --function-name video-processor-chunk-worker --payload '{}' --cli-binary-format raw-in-base64-out response.json
```

**Ver logs recentes:**
```bash
aws logs tail /aws/lambda/video-processor-chunk-worker --since 10m
```

**Listar funções Lambda:**
```bash
aws lambda list-functions
```

**Obter configuração da função:**
```bash
aws lambda get-function-configuration --function-name video-processor-chunk-worker
```

---

## 6. Próximos Passos

Após validar que o Lambda está funcionando:

1. ✅ Função Lambda responde com Hello World
2. ✅ Logs aparecem no CloudWatch
3. ⏭️ Implementar processamento real de vídeo
4. ⏭️ Integrar com S3 e Step Functions
5. ⏭️ Adicionar testes automatizados

Para dúvidas ou problemas, consulte a documentação AWS Lambda: https://docs.aws.amazon.com/lambda/
