# Storie-08: Reorganizar diretórios (Core, Infra, InterfacesExternas, Tests)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como mantenedor do repositório, quero reorganizar a estrutura de diretórios do código-fonte para refletir a Clean Architecture (~70%) e as convenções da rule core-clean-architecture, para facilitar navegação, onboarding e validação de dependências entre camadas.

## Objetivo
Reorganizar fisicamente os projetos da solution em pastas alinhadas à arquitetura (Core, Infra, InterfacesExternas, Tests), **sem alterar comportamento nem contrato da API** — apenas mudança de localização dos projetos, atualização da solution, referências, scripts e CI.

## Estrutura atual vs. alvo

**Atual:** Projetos em estrutura plana em `src/` e `tests/`:
- `src/VideoProcessor.Domain`, `src/VideoProcessor.Application`, `src/VideoProcessor.Infra`, `src/VideoProcessor.CLI`, `src/VideoProcessor.Lambda`
- `tests/VideoProcessor.Tests.Unit`, `tests/VideoProcessor.Tests.Bdd`

**Alvo (nomes reais desta codebase):**
- **`src/Core/`** → `VideoProcessor.Domain`, `VideoProcessor.Application`
- **`src/Infra/`** → `VideoProcessor.Infra`
- **`src/InterfacesExternas/`** → `VideoProcessor.CLI`, `VideoProcessor.Lambda`
- **`tests/`** → `VideoProcessor.Tests.Unit`, `VideoProcessor.Tests.Bdd` (permanecem em `tests/`)

Solution Folders na `.slnx`: Core, Infra, InterfacesExternas, Tests (espelhando as pastas físicas).

## Escopo Técnico
- **Tecnologias:** .NET 10, solution (.slnx), MSBuild, GitHub Actions
- **Arquivos afetados:**
  - `VideoProcessor.slnx` (paths dos projetos e Solution Folders)
  - Todos os `.csproj` (referências entre projetos, se necessário após movimentação)
  - `.github/workflows/deploy-lambda.yml` (path do projeto Lambda: `-pl`)
  - Scripts de build/deploy e documentação que citem caminhos (ex.: `docs/`, `storys/QUICK_START_GUIDE.md`, `docs/testelocalCli.md`, `src/VideoProcessor.Lambda/Readme.md`)
- **Componentes:** pastas físicas `src/Core`, `src/Infra`, `src/InterfacesExternas`; Solution Folders correspondentes
- **Pacotes/Dependências:** Nenhum pacote novo; apenas reorganização de paths

## Dependências e Riscos (para estimativa)
- **Dependências:** Nenhuma outra story obrigatória; recomenda-se fazer em branch dedicada.
- **Riscos:**
  - Referências quebradas em `.csproj` (ProjectReference) até paths serem atualizados
  - Solution (.slnx) com paths incorretos até a Subtask 03
  - CI (GitHub Actions) falhando até ajuste do path do projeto Lambda (Subtask 05)
  - Documentação e scripts desatualizados se não forem ajustados na Subtask 04

## Subtasks
- [x] [Subtask 01: Criar pastas físicas e mover projetos Core e Infra](./subtask/Subtask-01-Pastas_Core_Infra_Mover_Projetos.md)
- [x] [Subtask 02: Mover projetos InterfacesExternas](./subtask/Subtask-02-Mover_Projetos_InterfacesExternas.md)
- [x] [Subtask 03: Atualizar solution com Solution Folders e paths](./subtask/Subtask-03-Atualizar_Solution_Folders_Paths.md)
- [x] [Subtask 04: Ajustar referências, scripts e documentação](./subtask/Subtask-04-Ajustar_Referencias_Scripts_Docs.md)
- [x] [Subtask 05: Ajustar e validar workflows de CI](./subtask/Subtask-05-Ajustar_Validar_Workflows_CI.md)
- [x] [Subtask 06: Validar build e testes](./subtask/Subtask-06-Validar_Build_Testes.md)

## Critérios de Aceite da História
- [x] Pastas físicas existem: `src/Core/`, `src/Infra/`, `src/InterfacesExternas/`; projetos movidos conforme mapeamento (Domain e Application em Core; Infra em Infra; CLI e Lambda em InterfacesExternas; testes em `tests/`)
- [x] Solution (.slnx) contém Solution Folders Core, Infra, InterfacesExternas e Tests com paths corretos para cada projeto
- [x] `dotnet build` na raiz do repositório conclui com sucesso (Release)
- [x] `dotnet test` executa e todos os testes passam (Unit e Bdd)
- [x] Workflows de CI (ex.: GitHub Actions) atualizados para os novos paths (ex.: `-pl src/InterfacesExternas/VideoProcessor.Lambda`) e validados
- [x] Scripts de build/deploy e documentação que citem caminhos de projetos foram atualizados
- [x] Regras de dependência da Clean Architecture mantidas (Domain sem refs externas; Application só Domain; Infra e InterfacesExternas referenciam Application/Domain conforme já existente)

## Rastreamento (dev tracking)
- **Início:** 07/03/2026, às 13:43 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
