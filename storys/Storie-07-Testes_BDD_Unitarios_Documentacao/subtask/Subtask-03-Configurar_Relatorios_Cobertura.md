# Subtask 03: Configurar Relatórios de Cobertura e Workflow CI

## Descrição
Configurar workflow GitHub Actions para gerar relatórios de cobertura automaticamente em cada PR/push, publicar relatório como artifact, e adicionar badge de cobertura no README.

## Passos de Implementação
1. Criar `.github/workflows/test-coverage.yml`:
   ```yaml
   name: Test Coverage
   
   on:
     push:
       branches: [main, dev]
     pull_request:
       branches: [main, dev]
   
   jobs:
     test-coverage:
       runs-on: ubuntu-latest
       
       steps:
       - uses: actions/checkout@v4
       
       - name: Setup .NET 10
         uses: actions/setup-dotnet@v4
         with:
           dotnet-version: '10.0.x'
       
       - name: Restore dependencies
         run: dotnet restore
       
       - name: Build
         run: dotnet build --configuration Release --no-restore
       
       - name: Run tests with coverage
         run: dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory ./coverage
       
       - name: Install ReportGenerator
         run: dotnet tool install -g dotnet-reportgenerator-globaltool
       
       - name: Generate coverage report
         run: reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html
       
       - name: Upload coverage report
         uses: actions/upload-artifact@v4
         with:
           name: coverage-report
           path: ./coverage/report
       
       - name: Display coverage summary
         run: |
           echo "Coverage Report Generated"
           cat ./coverage/report/Summary.txt || echo "Summary not found"
   ```
2. Adicionar badge de cobertura no README (manual ou usando ferramenta):
   ```markdown
   ![Coverage](https://img.shields.io/badge/coverage-80%25-green)
   ```
   (Nota: badge manual; para auto-update, considerar integração com Codecov ou similar)
3. Atualizar README com seção "Como Rodar Testes e Ver Cobertura":
   ```markdown
   ## Testes e Cobertura
   
   ### Rodar todos os testes
   ```bash
   dotnet test
   ```
   
   ### Gerar relatório de cobertura local
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
   reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html
   open ./coverage/report/index.html  # ou start no Windows
   ```
   ```

## Formas de Teste
1. **Workflow local:** usar `act` para testar workflow localmente (se disponível)
2. **Push para branch:** fazer push e verificar que workflow executa e gera artifact
3. **Artifact:** baixar artifact e abrir `index.html` para verificar relatório

## Critérios de Aceite da Subtask
- [ ] Workflow `.github/workflows/test-coverage.yml` criado
- [ ] Workflow executa testes e gera relatório de cobertura em cada push/PR
- [ ] Relatório HTML publicado como artifact no GitHub Actions
- [ ] Badge de cobertura adicionado ao README (manual ou automático)
- [ ] README documenta como rodar testes e gerar relatório localmente
- [ ] Workflow executa com sucesso no GitHub Actions
