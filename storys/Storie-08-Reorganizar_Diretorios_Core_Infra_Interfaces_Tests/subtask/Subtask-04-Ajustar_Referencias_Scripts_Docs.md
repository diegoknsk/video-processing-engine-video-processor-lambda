# Subtask 04: Ajustar referências, scripts de build/deploy e documentação

## Descrição
Garantir que referências entre projetos (.csproj), scripts de build/deploy e documentação que citem caminhos de projetos usem os novos paths (src/Core, src/Infra, src/InterfacesExternas, tests).

## Passos de implementação
1. **Referências em .csproj:** Verificar se algum `.csproj` usa caminhos absolutos ou relativos hardcoded para outros projetos. Em soluções padrão, `ProjectReference` usa apenas o path do .csproj referenciado; após a movimentação, a solution já resolve pelos paths atualizados na Subtask 03. Se houver referências explícitas a paths antigos em algum .csproj, atualizá-las.
2. **Scripts:** Procurar por scripts (`.sh`, `.ps1`, `Makefile`, etc.) na raiz ou em `scripts/` que referenciem `src/VideoProcessor.*` ou `tests/VideoProcessor.*` e atualizar para os novos paths (ex.: `src/InterfacesExternas/VideoProcessor.Lambda`, `src/Core/VideoProcessor.Application`).
3. **Documentação:** Buscar em `docs/`, `storys/`, `README.md` e arquivos como `QUICK_START_GUIDE.md`, `testelocalCli.md`, `Readme.md` dentro de projetos por menções a caminhos antigos (ex.: `src/VideoProcessor.Lambda`, `src/VideoProcessor.CLI`) e atualizar para `src/InterfacesExternas/VideoProcessor.Lambda`, `src/InterfacesExternas/VideoProcessor.CLI`, `src/Core/VideoProcessor.Application`, etc., conforme aplicável.
4. Registrar em comentário ou na story quais arquivos foram alterados para referência futura.

## Formas de teste
- Grep por padrões como `src/VideoProcessor.` (sem `Core/`, `Infra/`, `InterfacesExternas/`) em docs e scripts: não deve restar referência aos paths antigos de projetos movidos.
- Executar qualquer script de build/deploy existente e verificar que não quebra por path incorreto.
- Leitura rápida da documentação atualizada para garantir coerência.

## Critérios de aceite
- [x] Nenhum .csproj referencia path antigo dos projetos movidos (apenas paths relativos corretos ou resolução via solution).
- [x] Scripts de build/deploy que usem caminhos de projetos foram atualizados para a nova estrutura.
- [x] Documentação que citava caminhos de projetos (ex.: comandos `dotnet run --project ...`, `dotnet test ...`) foi atualizada com os novos paths.
- [x] Lista dos arquivos alterados (scripts + docs) documentada na subtask ou na story.

## Arquivos alterados (Subtask 04)
- **.csproj (referências):** `src/Infra/VideoProcessor.Infra/VideoProcessor.Infra.csproj`, `src/InterfacesExternas/VideoProcessor.CLI/VideoProcessor.CLI.csproj`, `src/InterfacesExternas/VideoProcessor.Lambda/VideoProcessor.Lambda.csproj`, `tests/VideoProcessor.Tests.Unit/VideoProcessor.Tests.Unit.csproj`, `tests/VideoProcessor.Tests.Bdd/VideoProcessor.Tests.Bdd.csproj`
- **Documentação:** `docs/testelocalCli.md`, `storys/QUICK_START_GUIDE.md`, `src/InterfacesExternas/VideoProcessor.Lambda/Readme.md`
- **CI:** `.github/workflows/deploy-lambda.yml` (path do projeto Lambda no package)
