# Subtask 01: Criar Workflow GitHub Actions para Build e Deploy

## Descrição
Criar workflow GitHub Actions (`.github/workflows/deploy-lambda.yml`) que realiza checkout do código, configura .NET 10, executa build, roda testes, gera ZIP do Lambda e atualiza a função na AWS usando credenciais temporárias armazenadas como secrets.

## Passos de Implementação
1. Criar `.github/workflows/deploy-lambda.yml` com trigger em push para branch `main` ou `dev`
2. Definir jobs:
   - **Job: build-and-test**
     - Checkout do código (`actions/checkout@v4`)
     - Setup .NET 10 (`actions/setup-dotnet@v4` com version 10.0.x)
     - Restore: `dotnet restore`
     - Build: `dotnet build --configuration Release --no-restore`
     - Test: `dotnet test --no-build --verbosity normal`
   - **Job: deploy** (depends on: build-and-test)
     - Checkout do código
     - Setup .NET 10
     - Build Release: `dotnet build -c Release`
     - Package Lambda: `dotnet lambda package -o artifacts/VideoProcessor.zip -c Release` (ou publish + zip)
     - Configure AWS credentials:
       ```yaml
       - name: Configure AWS credentials
         uses: aws-actions/configure-aws-credentials@v4
         with:
           aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
           aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
           aws-session-token: ${{ secrets.AWS_SESSION_TOKEN }}
           aws-region: ${{ secrets.AWS_REGION }}
       ```
     - Update Lambda function:
       ```bash
       aws lambda update-function-code \
         --function-name video-processor-chunk-worker \
         --zip-file fileb://artifacts/VideoProcessor.zip
       ```
     - Wait for update to complete:
       ```bash
       aws lambda wait function-updated \
         --function-name video-processor-chunk-worker
       ```
3. Adicionar upload de artifacts (ZIP) para debugging:
   ```yaml
   - name: Upload Lambda package
     uses: actions/upload-artifact@v4
     with:
       name: lambda-package
       path: artifacts/VideoProcessor.zip
   ```

## Formas de Teste
1. **Sintaxe YAML:** usar extensão do VS Code ou `yamllint` para validar sintaxe
2. **Dry-run local:** usar `act` (GitHub Actions local runner) se disponível
3. **Push para branch de teste:** criar branch temporária e fazer push para verificar execução no GitHub Actions

## Critérios de Aceite da Subtask
- [ ] Arquivo `.github/workflows/deploy-lambda.yml` criado com estrutura completa
- [ ] Job `build-and-test` executa build e testes com sucesso
- [ ] Job `deploy` atualiza função Lambda usando `aws lambda update-function-code`
- [ ] Workflow usa secrets para credenciais AWS (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, AWS_REGION)
- [ ] Artifact do ZIP é salvo no GitHub Actions para download manual (debugging)
- [ ] Workflow executa apenas após testes passarem (job dependency configurada)
