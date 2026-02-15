# Subtask 02: Adicionar Testes Unitários Faltantes para Cobertura ≥ 80%

## Descrição
Identificar gaps de cobertura de testes, adicionar testes unitários faltantes para classes/métodos não cobertos, e garantir cobertura global ≥ 80% em todos os projetos (exceto Lambda bootstrap e testes).

## Passos de Implementação
1. Executar relatório de cobertura atual:
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
   ```
2. Analisar relatório com ReportGenerator (instalar se necessário):
   ```bash
   dotnet tool install -g dotnet-reportgenerator-globaltool
   reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html
   ```
3. Abrir `./coverage/report/index.html` e identificar classes/métodos não cobertos
4. Adicionar testes unitários faltantes:
   - **S3Service:** testar cenários de erro (AmazonS3Exception), JSON inválido na deserialização
   - **PrefixBuilder:** casos de borda (prefixos vazios, múltiplos trailing slashes)
   - **ErrorClassifier:** todas as exceções conhecidas e desconhecidas
   - **ContractVersionValidator:** versões edge case
   - **CorrelationContext:** AsyncLocal propagation
   - **MetricsPublisher:** formato EMF correto
   - **LoggerExtensions:** scope criado corretamente
5. Para classes difíceis de testar (ex.: Lambda Function.cs bootstrap), considerar adicionar `[ExcludeFromCodeCoverage]` em métodos puramente de DI se apropriado
6. Re-executar cobertura e verificar que atingiu ≥ 80%

## Formas de Teste
1. **Cobertura atual:** analisar relatório HTML
2. **Identificar gaps:** verificar classes/métodos com 0% ou < 50% de cobertura
3. **Validar testes:** cada teste adicionado deve passar e aumentar cobertura

## Critérios de Aceite da Subtask
- [ ] Relatório de cobertura gerado com ReportGenerator
- [ ] Cobertura global ≥ 80% (excluindo projetos de teste e Lambda bootstrap)
- [ ] Testes adicionados para classes identificadas com baixa cobertura
- [ ] Todos os testes passam: `dotnet test`
- [ ] Relatório HTML mostra cobertura ≥ 80% em todos os projetos de produção (Domain, Application, Infra)
