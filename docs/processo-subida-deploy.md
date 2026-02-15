# Processo de subida – Deploy Lambda (GitHub Actions)

Este documento registra o **processo de subida** e **todas as variáveis e secrets** que precisam ser configurados para o deploy do **Video Processor Lambda** via GitHub Actions.

---

## 1. Checklist de configuração

### 1.1 GitHub Secrets (obrigatórios para deploy)

Configurar em: **Settings** → **Secrets and variables** → **Actions** → **Secrets** → **New repository secret**.

| Secret | Descrição | Obrigatório |
|--------|-----------|-------------|
| **AWS_ACCESS_KEY_ID** | Access Key ID do IAM User (ou credenciais temporárias STS/AWS Academy) | Sim |
| **AWS_SECRET_ACCESS_KEY** | Secret Access Key correspondente | Sim |
| **AWS_SESSION_TOKEN** | Token de sessão (obrigatório para credenciais temporárias do AWS Academy) | Sim* |
| **AWS_REGION** | Região AWS onde o Lambda está (ex.: `us-east-1`) | Sim |

\* **AWS_SESSION_TOKEN:** obrigatório quando usar credenciais temporárias (AWS Academy, AssumeRole, STS). Se usar IAM User permanente, pode deixar vazio ou criar o secret com valor vazio.

### 1.2 GitHub Variables (opcionais)

O workflow atual **não usa Variables** — o nome da função e a região vêm dos **Secrets**. Se quiser parametrizar no futuro:

| Variable | Descrição | Valor atual (hardcoded) |
|----------|-----------|-------------------------|
| **AWS_REGION** | Região AWS | Usa `secrets.AWS_REGION` |
| **LAMBDA_FUNCTION_NAME** | Nome da função Lambda | `video-processor-chunk-worker` |

> **Nota:** O workflow `.github/workflows/deploy-lambda.yml` usa o nome fixo `video-processor-chunk-worker`. Para alterar, edite o arquivo do workflow ou parametrize com Variables.

---

## 2. Resumo do que configurar

| Onde | O que setar |
|------|-------------|
| **GitHub Secrets** | `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION` |
| **GitHub Secrets** (credenciais temporárias) | + `AWS_SESSION_TOKEN` |
| **AWS** | Função Lambda criada com nome `video-processor-chunk-worker` na região configurada |

### Handler da função Lambda

O Handler deve ser configurado na criação da função (IaC ou Console):

```
VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler
```

---

## 3. Credenciais temporárias (AWS Academy / STS)

Credenciais temporárias do **AWS Academy** expiram periodicamente. Para renovar:

1. Acesse o AWS Academy e gere novas credenciais temporárias
2. Em **GitHub** → **Settings** → **Secrets and variables** → **Actions** → **Secrets**, atualize:
   - **AWS_ACCESS_KEY_ID**
   - **AWS_SECRET_ACCESS_KEY**
   - **AWS_SESSION_TOKEN**
3. O próximo push disparará o pipeline com as novas credenciais

O workflow já está preparado: o step **Configure AWS credentials** usa `secrets.AWS_SESSION_TOKEN`. O token é passado corretamente para a action `aws-actions/configure-aws-credentials@v4`.

---

## 4. Permissões IAM necessárias

O IAM User ou Role das credenciais precisa das seguintes permissões:

| Permissão | Uso |
|-----------|-----|
| `lambda:UpdateFunctionCode` | Atualizar o código do Lambda |
| `lambda:GetFunction` | Verificar se a função existe |
| `lambda:GetFunctionConfiguration` | Aguardar atualização (`aws lambda wait function-updated`) |

Política mínima sugerida:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "lambda:UpdateFunctionCode",
        "lambda:GetFunction",
        "lambda:GetFunctionConfiguration"
      ],
      "Resource": "arn:aws:lambda:*:*:function:video-processor-chunk-worker"
    }
  ]
}
```

---

## 5. Processo de subida (passo a passo)

### 5.1 Pré-requisitos

1. **Repositório**
   - Secrets configurados: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`
   - Se usar credenciais temporárias: `AWS_SESSION_TOKEN`

2. **AWS**
   - Função Lambda criada com nome `video-processor-chunk-worker`
   - Handler configurado: `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler`
   - Runtime: .NET 10 (ou compatível)

### 5.2 Deploy automático

O workflow é disparado automaticamente em **push** para as branches:

- `main`
- `dev`

```bash
git add .
git commit -m "feat: sua mensagem"
git push origin dev
```

### 5.3 Etapas do pipeline

| Step | Descrição |
|------|-----------|
| **build-and-test** | Restore → Build Release → Test |
| **deploy** | Package Lambda → Configure AWS credentials → Update Lambda function → Wait → Upload artifact |

### 5.4 Verificação

1. Acesse **GitHub** → **Actions** e verifique se o workflow **Deploy Lambda** concluiu com sucesso
2. Procure pela mensagem: `Lambda function updated successfully`
3. No **AWS Console** → **Lambda** → **Functions** → `video-processor-chunk-worker`:
   - Verifique **Last modified** (deve ser recente)
   - Use a aba **Test** para invocar e validar

---

## 6. Referência rápida (tabela única)

| Tipo | Nome | Obrigatório | Observação |
|------|------|-------------|------------|
| Secret | AWS_ACCESS_KEY_ID | Sim | Deploy |
| Secret | AWS_SECRET_ACCESS_KEY | Sim | Deploy |
| Secret | AWS_REGION | Sim | Ex.: `us-east-1` |
| Secret | AWS_SESSION_TOKEN | Sim* | *Obrigatório para credenciais temporárias (AWS Academy) |
| — | LAMBDA_FUNCTION_NAME | — | Fixo no workflow: `video-processor-chunk-worker` |

---

## 7. Troubleshooting

| Problema | Solução |
|---------|---------|
| **"The security token included in the request is expired"** | Renove as credenciais no AWS Academy e atualize os Secrets |
| **"Function not found"** | Verifique se o nome da função é `video-processor-chunk-worker` e se a região está correta |
| **Build/Test falha** | Verifique os logs do step `build-and-test`; rode `dotnet build` e `dotnet test` localmente |
| **Deploy falha no step "Configure AWS credentials"** | Verifique se os Secrets estão preenchidos corretamente (sem espaços extras) |

Documentação adicional: [INVOCATION_GUIDE.md](./INVOCATION_GUIDE.md) (como invocar o Lambda após o deploy).
