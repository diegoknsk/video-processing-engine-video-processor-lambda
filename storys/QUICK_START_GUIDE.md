# Quick Start Guide — Desenvolvedor

## 🚀 Começando Agora

Você está prestes a desenvolver o **Lambda Video Processor**, um worker minimalista .NET 10 que processa chunks de vídeo no pipeline da Step Functions.

### O Que Você Precisa Saber

**Arquitetura:** Handler Lambda puro (sem API Gateway, sem AddAWSLambdaHosting)  
**Paradigma:** Clean Architecture (Domain → Application → Infra → Lambda)  
**Input:** JSON com videoId, chunk (startSec, endSec), source (S3), output (S3)  
**Output:** JSON com status (SUCCEEDED/FAILED), manifest, error  
**Idempotência:** done.json marker no S3  
**Observabilidade:** Logs estruturados + métricas customizadas  

---

## 📋 Pré-requisitos

### Instalar Antes de Começar
- [ ] .NET 10 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- [ ] AWS CLI ([Guia de Instalação](https://aws.amazon.com/cli/))
- [ ] Git
- [ ] IDE: Visual Studio 2022+ ou VS Code com extensão C#

### Configurar AWS Academy
1. Acessar AWS Academy Learner Lab
2. Clicar em "AWS Details" → "AWS CLI"
3. Copiar credenciais temporárias:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `AWS_SESSION_TOKEN`
4. Configurar localmente:
   ```bash
   aws configure set aws_access_key_id <YOUR_KEY>
   aws configure set aws_secret_access_key <YOUR_SECRET>
   aws configure set aws_session_token <YOUR_TOKEN>
   aws configure set region us-east-1
   ```

### Configurar GitHub Secrets
1. Ir para Settings → Secrets and variables → Actions
2. Adicionar secrets:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `AWS_SESSION_TOKEN`
   - `AWS_REGION` (ex.: us-east-1)

---

## 📖 Lendo as Stories

### Estrutura de Navegação
```
storys/
├── INDEX.md                    ← Índice geral (comece aqui)
├── EXECUTIVE_SUMMARY.md        ← Resumo executivo (visão geral)
├── VALIDATION_CHECKLIST.md     ← Checklist de validação
├── Storie-01-.../story.md      ← Story principal
│   └── subtask/
│       ├── Subtask-01-....md   ← Subtask detalhada
│       └── ...
└── ...
```

### Ordem de Leitura Recomendada
1. **INDEX.md** — Entenda o escopo geral e ordem de execução
2. **EXECUTIVE_SUMMARY.md** — Veja estatísticas, riscos e cronograma
3. **Storie-01/story.md** — Leia a primeira story completa
4. **Storie-01/subtask/Subtask-01-*.md** — Leia as subtasks sequencialmente

---

## 🛠️ Desenvolvendo a Primeira Story

### Storie-01: Bootstrap do Projeto

#### Passo 1: Ler a Story
```bash
# Abrir em seu editor
code storys/Storie-01-Bootstrap_Projeto_Lambda_NET10/story.md
```

#### Passo 2: Registrar Início
Conforme skill de technical-stories, registrar início no `story.md`:
```bash
# Obter horário atual (Brasília)
powershell -Command "Get-Date -Format 'dd/MM/yyyy HH:mm'"
```

Atualizar seção **Rastreamento (dev tracking)**:
```markdown
- **Início:** dia 14/02/2026, às 20:56 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
```

#### Passo 3: Executar Subtasks Sequencialmente
Abrir e seguir cada subtask em ordem:

**Subtask-01: Criar estrutura de projetos**
```bash
# Criar global.json
echo '{ "sdk": { "version": "10.0.100", "rollForward": "latestFeature" } }' > global.json

# Criar solution
dotnet new sln -n VideoProcessor

# Criar projetos (seguir comandos da subtask)
...
```

**Subtask-02: Implementar handler**
```bash
# Seguir passos na subtask
# Instalar pacotes
cd src/InterfacesExternas/VideoProcessor.Lambda
dotnet add package Amazon.Lambda.Core --version 2.3.0
...
```

**Subtask-03: Configurar empacotamento**
```bash
# Testar empacotamento
dotnet lambda package -o artifacts/VideoProcessor.zip
```

**Subtask-04: Criar testes**
```bash
# Executar testes
dotnet test
```

#### Passo 4: Validar Critérios de Aceite
Ir ao `story.md` e marcar cada critério:
```markdown
- [x] Solution com 5 projetos compilando sem erros
- [x] Handler Lambda com método FunctionHandler
- [ ] ...
```

#### Passo 5: Registrar Conclusão
```bash
# Obter horário final
powershell -Command "Get-Date -Format 'dd/MM/yyyy HH:mm'"
```

Atualizar `story.md`:
```markdown
## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 14/02/2026

## Rastreamento (dev tracking)
- **Início:** dia 14/02/2026, às 20:56 (Brasília)
- **Fim:** dia 15/02/2026, às 01:30 (Brasília)
- **Tempo total de desenvolvimento:** 4h 34min
```

---

## 🧪 Testando Localmente

### Executar Testes
```bash
# Todos os testes
dotnet test

# Apenas unitários
dotnet test tests/VideoProcessor.Tests.Unit

# Apenas BDD (após Storie-07)
dotnet test tests/VideoProcessor.Tests.Bdd

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Testar Handler Localmente
```bash
# Criar console app de teste
dotnet new console -n VideoProcessor.LocalTest

# Adicionar referência ao Lambda
dotnet add reference ../src/InterfacesExternas/VideoProcessor.Lambda

# Implementar teste (ver exemplos nas subtasks)
```

---

## 🚢 Deploy via GitHub Actions

### Primeira Vez (após Storie-01 e Storie-02)
```bash
# 1. Commitar código
git add .
git commit -m "Storie-01: Bootstrap do projeto Lambda"

# 2. Push para branch dev ou main
git push origin dev

# 3. Acompanhar no GitHub Actions
# https://github.com/<seu-repo>/actions
```

### Deploy Manual (emergência)
```bash
# Empacotar
dotnet lambda package -o artifacts/VideoProcessor.zip

# Deploy via AWS CLI
aws lambda update-function-code \
  --function-name video-processor-chunk-worker \
  --zip-file fileb://artifacts/VideoProcessor.zip
```

---

## 📊 Monitorando Logs e Métricas

### CloudWatch Logs
```bash
# Tail logs em tempo real
aws logs tail /aws/lambda/video-processor-chunk-worker --follow

# Query Logs Insights (após Storie-06)
# Abrir console AWS → CloudWatch → Logs Insights
# Query:
fields @timestamp, @message, videoId, chunkId, status
| filter videoId = "test-video-123"
| sort @timestamp desc
```

### CloudWatch Metrics
```bash
# Abrir console AWS → CloudWatch → Metrics
# Namespace: VideoProcessing/ChunkWorker
# Métricas: ProcessingDuration, ProcessingResult, ChunksProcessed
```

---

## 🐛 Troubleshooting Rápido

### Problema: Credenciais AWS Expiradas
**Solução:** Renovar no AWS Academy e atualizar secrets GitHub

### Problema: Testes Falhando
**Solução:** 
```bash
dotnet clean
dotnet restore
dotnet build
dotnet test
```

### Problema: Deploy Falha no GitHub Actions
**Solução:** Verificar logs do workflow; confirmar que secrets estão configurados

### Problema: Lambda Não Aparece Logs
**Solução:** Verificar permissões IAM do Lambda (logs:CreateLogStream, logs:PutLogEvents)

**Mais detalhes:** Ver `docs/TROUBLESHOOTING.md` (criado na Storie-07)

---

## 📚 Documentos de Apoio

### Durante Desenvolvimento
- **INDEX.md** — Navegação entre stories
- **story.md** de cada story — Contexto e critérios
- **Subtask-XX-*.md** — Passos detalhados
- **.cursor/skills/** — Skills técnicas do projeto (Clean Architecture, FluentValidation, etc.)

### Após Desenvolvimento (Storie-07)
- **README.md** (raiz) — Como buildar, testar, deployar
- **docs/architecture/ADR-*.md** — Decisões arquiteturais
- **docs/TROUBLESHOOTING.md** — Problemas comuns
- **docs/RUNBOOK.md** — Operação e monitoramento
- **docs/S3_CONVENTIONS.md** — Convenções de prefixos S3

---

## 🎯 Dicas de Produtividade

### 1. Use Aliases Git
```bash
git config alias.st status
git config alias.co checkout
git config alias.br branch
git config alias.ci commit
```

### 2. Configure Watch para Testes
```bash
# Terminal 1: Watch de testes
dotnet watch test --project tests/VideoProcessor.Tests.Unit

# Terminal 2: Desenvolvimento
code .
```

### 3. Use Snippets de Código
Criar snippets para records, validators, use cases (C# snippets no VS Code)

### 4. Mantenha Dev Tracking Atualizado
Sempre registrar início/fim conforme skill de technical-stories

---

## 🏁 Checklist Antes de Marcar Story como Pronta

- [ ] Todos os critérios de aceite marcados como [x]
- [ ] Todos os testes passando (`dotnet test`)
- [ ] Código commitado e pushed
- [ ] Deploy via GitHub Actions bem-sucedido (a partir da Storie-02)
- [ ] Dev tracking preenchido (início, fim, duração)
- [ ] Status da story atualizado para "✅ Concluída"
- [ ] Data de conclusão preenchida

---

## 💬 Comunicação com o Time

### Ao Começar uma Story
"Iniciando Storie-XX: [Nome da Story]. Previsão: [X] horas."

### Ao Concluir uma Story
"Storie-XX concluída! Duração: [X]h [Y]min. Próxima: Storie-[XX+1]."

### Ao Encontrar Bloqueio
"Bloqueio na Storie-XX, Subtask-YY: [descrever problema]. Necessito de: [ajuda específica]."

---

## 📞 Suporte

### Recursos
- **Skills do Projeto:** `.cursor/skills/`
- **Rules do Projeto:** `.cursor/rules/`
- **Stories:** `storys/`
- **Comunidade .NET:** [docs.microsoft.com](https://docs.microsoft.com/dotnet)
- **AWS Lambda Docs:** [aws.amazon.com/lambda](https://aws.amazon.com/lambda)

---

**Boa sorte com o desenvolvimento! 🚀**

Comece pelo **INDEX.md** para entender a jornada completa, depois mergulhe na **Storie-01**.

Lembre-se: seguir as subtasks sequencialmente é a chave para o sucesso!
