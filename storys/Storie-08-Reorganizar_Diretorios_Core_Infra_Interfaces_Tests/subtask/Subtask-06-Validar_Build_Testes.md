# Subtask 06: Validar build e testes

## Descrição
Executar build e testes na raiz do repositório para garantir que não há referências quebradas e que a reorganização não introduziu regressões. Validar que as regras de dependência da Clean Architecture continuam respeitadas.

## Passos de implementação
1. Na raiz do repositório: `dotnet restore`.
2. `dotnet build --configuration Release` — deve concluir com sucesso para toda a solution.
3. `dotnet test --no-build --configuration Release --verbosity normal` — todos os testes (Unit e Bdd) devem passar.
4. Opcional: validar dependências entre projetos (ex.: Domain não referencia outros projetos; Application referencia apenas Domain; Infra e InterfacesExternas referenciam Application/Domain conforme esperado). Isso pode ser feito por inspeção dos `.csproj` ou ferramenta de análise de dependências.
5. Registrar o resultado (build OK, testes OK) e marcar a story como pronta para conclusão.

## Formas de teste
- Build: executar `dotnet build` e verificar exit code 0 e ausência de erros de compilação ou projeto não encontrado.
- Testes: executar `dotnet test` e verificar que todos os testes passam (contagem de passed/failed nos relatórios).
- Smoke: executar a CLI a partir do novo path, ex.: `dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --help` (ou comando mínimo) para garantir que o projeto inicia.

## Critérios de aceite
- [x] `dotnet restore` e `dotnet build --configuration Release` na raiz concluem com sucesso.
- [x] `dotnet test --no-build --configuration Release` executa e todos os testes passam (0 falhas).
- [x] Não há referências quebradas (erros de tipo "project not found" ou assembly não encontrado).
- [x] Regras de dependência da arquitetura mantidas (Domain sem dependências de outros projetos da solution; Application apenas Domain; Infra e InterfacesExternas referenciando Core conforme já existente).
