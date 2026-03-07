# Subtask 03: Atualizar solution com Solution Folders e paths

## Descrição
Atualizar o arquivo da solution (`.slnx`) para refletir a nova estrutura: Solution Folders Core, Infra, InterfacesExternas e Tests, e paths corretos para cada projeto.

## Passos de implementação
1. Abrir `VideoProcessor.slnx` na raiz do repositório.
2. Substituir a estrutura atual (pasta única `/src/` com todos os projetos) por:
   - **Solution Folder "Core"** com: `src/Core/VideoProcessor.Domain/VideoProcessor.Domain.csproj`, `src/Core/VideoProcessor.Application/VideoProcessor.Application.csproj`
   - **Solution Folder "Infra"** com: `src/Infra/VideoProcessor.Infra/VideoProcessor.Infra.csproj`
   - **Solution Folder "InterfacesExternas"** com: `src/InterfacesExternas/VideoProcessor.CLI/VideoProcessor.CLI.csproj`, `src/InterfacesExternas/VideoProcessor.Lambda/VideoProcessor.Lambda.csproj`
   - **Solution Folder "Tests"** com: `tests/VideoProcessor.Tests.Unit/VideoProcessor.Tests.Unit.csproj`, `tests/VideoProcessor.Tests.Bdd/VideoProcessor.Tests.Bdd.csproj`
3. Garantir que os paths são relativos à raiz do repositório e que a sintaxe do `.slnx` está correta (tags `<Folder>`, `<Project Path="...">`).
4. Salvar e executar `dotnet restore` e `dotnet build` na raiz; corrigir paths se o build indicar projeto não encontrado.

## Formas de teste
- `dotnet restore` na raiz deve concluir sem erro.
- `dotnet build --configuration Release` na raiz deve compilar todos os projetos.
- No IDE, a solution deve exibir as pastas Core, Infra, InterfacesExternas e Tests com os projetos corretos dentro de cada uma.

## Critérios de aceite
- [x] O arquivo `.slnx` contém quatro Solution Folders: Core, Infra, InterfacesExternas, Tests.
- [x] Cada projeto está listado sob o folder correto e com path relativo correto (ex.: `src/Core/VideoProcessor.Domain/VideoProcessor.Domain.csproj`).
- [x] `dotnet build` na raiz conclui com sucesso (sem erros de projeto não encontrado).
