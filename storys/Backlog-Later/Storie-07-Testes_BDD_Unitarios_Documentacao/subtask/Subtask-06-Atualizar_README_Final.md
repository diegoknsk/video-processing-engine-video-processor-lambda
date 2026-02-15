# Subtask 06: Atualizar README Final com Todas as Seções

## Descrição
Atualizar README.md com estrutura completa documentando: visão geral do projeto, arquitetura, pré-requisitos, como buildar/testar/deployar, estrutura de pastas, observabilidade, troubleshooting, e links para documentação detalhada.

## Passos de Implementação
1. Atualizar `README.md` com estrutura completa:
   ```markdown
   # Video Processor Lambda — Chunk Worker
   
   ![Coverage](https://img.shields.io/badge/coverage-80%25-green)
   ![.NET](https://img.shields.io/badge/.NET-10.0-blue)
   ![AWS Lambda](https://img.shields.io/badge/AWS-Lambda-orange)
   
   ## Visão Geral
   
   Lambda Worker minimalista para processar chunks individuais de vídeo no pipeline distribuído de processamento. Executado no **Map** da Step Functions (fan-out/fan-in), processa **APENAS 1 chunk** por invocação, gerando manifestos e frames (mockados no MVP).
   
   **Características:**
   - Handler puro .NET 10 (sem AddAWSLambdaHosting)
   - Idempotente (done marker no S3)
   - Classificação de erros retryable vs não-retryable
   - Logs estruturados com correlation (videoId, chunkId, executionArn)
   - Métricas customizadas (duração, sucesso/falha)
   
   ## Arquitetura
   
   ```
   Step Functions Map → Lambda Worker → S3 (manifest.json, done.json)
   ```
   
   **Componentes:**
   - **Handler:** Function.cs (puro, sem API Gateway)
   - **Use Case:** ProcessChunkUseCase (lógica de negócio)
   - **Ports:** IS3Service, IMetricsPublisher (Clean Architecture)
   - **Infra:** S3Service, CloudWatchMetricsPublisher
   
   **Decisões Técnicas:** Ver [docs/architecture/](docs/architecture/)
   
   ## Pré-requisitos
   
   - .NET 10 SDK
   - AWS CLI configurado
   - Credenciais temporárias AWS Academy (ACCESS_KEY_ID, SECRET_ACCESS_KEY, SESSION_TOKEN)
   - Bucket S3 para output
   
   ## Como Buildar
   
   ```bash
   dotnet restore
   dotnet build --configuration Release
   ```
   
   ## Como Rodar Testes
   
   ### Todos os testes (Unit + BDD)
   ```bash
   dotnet test
   ```
   
   ### Apenas testes unitários
   ```bash
   dotnet test tests/VideoProcessor.Tests.Unit
   ```
   
   ### Apenas testes BDD
   ```bash
   dotnet test tests/VideoProcessor.Tests.Bdd
   ```
   
   ### Gerar relatório de cobertura
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
   reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html
   open ./coverage/report/index.html
   ```
   
   ## Como Empacotar
   
   ```bash
   dotnet lambda package -o artifacts/VideoProcessor.zip -c Release
   ```
   
   ## Como Fazer Deploy
   
   ### Via GitHub Actions (Recomendado)
   1. Configurar secrets no GitHub: AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, AWS_REGION
   2. Push para branch `main` ou `dev`
   3. Workflow executa build, testes e deploy automaticamente
   
   ### Manual via AWS CLI
   ```bash
   aws lambda update-function-code \
     --function-name video-processor-chunk-worker \
     --zip-file fileb://artifacts/VideoProcessor.zip
   ```
   
   ## Estrutura do Projeto
   
   ```
   src/
   ├── VideoProcessor.Lambda/        # Handler puro
   ├── VideoProcessor.Application/   # Use cases, validators, services
   ├── VideoProcessor.Domain/        # Models, ports, exceptions
   └── VideoProcessor.Infra/         # S3Service, MetricsPublisher
   tests/
   ├── VideoProcessor.Tests.Unit/    # Testes unitários (xUnit)
   └── VideoProcessor.Tests.Bdd/     # Testes BDD (SpecFlow)
   docs/
   ├── architecture/                 # ADRs
   ├── TROUBLESHOOTING.md
   ├── RUNBOOK.md
   └── S3_CONVENTIONS.md
   ```
   
   ## Observabilidade
   
   ### Logs Estruturados
   Logs incluem automaticamente: videoId, chunkId, executionArn, correlationId
   
   **Query CloudWatch Logs Insights:**
   ```
   fields @timestamp, @message, videoId, chunkId, status
   | filter videoId = "your-video-id"
   | sort @timestamp desc
   ```
   
   ### Métricas Customizadas
   Namespace: `VideoProcessing/ChunkWorker`
   - **ProcessingDuration:** duração em ms
   - **ProcessingResult:** count por status (SUCCEEDED/FAILED)
   - **ChunksProcessed:** count total
   
   ## Troubleshooting
   
   Ver [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) para problemas comuns e soluções.
   
   **Problemas frequentes:**
   - Credenciais AWS Academy expiradas
   - Validação de input falha
   - Done marker órfão
   - Logs não aparecem
   - Timeout
   
   ## Operação
   
   Ver [docs/RUNBOOK.md](docs/RUNBOOK.md) para runbook operacional completo.
   
   ## CI/CD
   
   - **Build e Testes:** GitHub Actions em cada push/PR
   - **Deploy:** Automático em push para `main`/`dev`
   - **Cobertura:** Relatório gerado automaticamente
   
   ## Licença
   
   [Inserir licença se aplicável]
   
   ## Contribuindo
   
   1. Fork o projeto
   2. Criar feature branch (`git checkout -b feature/nova-funcionalidade`)
   3. Commit mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
   4. Push para branch (`git push origin feature/nova-funcionalidade`)
   5. Criar Pull Request
   ```
2. Verificar que todos os links internos estão corretos
3. Adicionar badges (coverage, .NET version, AWS Lambda) no topo
4. Revisar formatação Markdown

## Formas de Teste
1. **Validação de links:** verificar que todos os links internos (`docs/...`) apontam para arquivos existentes
2. **Renderização:** visualizar README no GitHub (ou usar extensão Markdown Preview)
3. **Completude:** verificar que cobre todas as seções necessárias (build, test, deploy, troubleshoot)

## Critérios de Aceite da Subtask
- [ ] README.md atualizado com estrutura completa
- [ ] Seções incluem: Visão Geral, Arquitetura, Pré-requisitos, Como Buildar, Como Testar, Como Deployar, Estrutura do Projeto, Observabilidade, Troubleshooting, Operação, CI/CD
- [ ] Badges adicionados: coverage, .NET version, AWS Lambda
- [ ] Todos os links internos funcionam (apontam para arquivos existentes)
- [ ] Comandos de exemplo testados e funcionais
- [ ] Formatação Markdown correta (listas, code blocks, links)
- [ ] README renderiza corretamente no GitHub
