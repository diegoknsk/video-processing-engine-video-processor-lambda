# Subtask 01: Criar Estrutura de Projetos e Solution

## Descrição
Criar a solution .NET com estrutura de projetos seguindo Clean Architecture (Domain, Application, Infra, Lambda, Tests), configurar referências entre projetos, e criar `global.json` para fixar .NET 10 SDK.

## Passos de Implementação
1. Criar `global.json` na raiz com SDK .NET 10:
   ```json
   { "sdk": { "version": "10.0.100", "rollForward": "latestFeature" } }
   ```
2. Criar solution: `dotnet new sln -n VideoProcessor`
3. Criar projetos:
   - `dotnet new classlib -n VideoProcessor.Domain -o src/VideoProcessor.Domain -f net10.0`
   - `dotnet new classlib -n VideoProcessor.Application -o src/VideoProcessor.Application -f net10.0`
   - `dotnet new classlib -n VideoProcessor.Infra -o src/VideoProcessor.Infra -f net10.0`
   - `dotnet new lambda.EmptyFunction -n VideoProcessor.Lambda -o src/VideoProcessor.Lambda -f net10.0`
   - `dotnet new xunit -n VideoProcessor.Tests.Unit -o tests/VideoProcessor.Tests.Unit -f net10.0`
   - `dotnet new xunit -n VideoProcessor.Tests.Bdd -o tests/VideoProcessor.Tests.Bdd -f net10.0`
4. Adicionar projetos à solution: `dotnet sln add src/**/*.csproj tests/**/*.csproj`
5. Configurar referências entre projetos:
   - Lambda → Application, Infra
   - Application → Domain
   - Infra → Domain, Application
   - Tests.Unit → Domain, Application, Infra
   - Tests.Bdd → Lambda
6. Remover arquivos template desnecessários (Class1.cs, Function.cs gerado pelo template)

## Formas de Teste
1. **Build da solution:** `dotnet build` deve compilar todos os projetos sem erros
2. **Validação de referências:** `dotnet list reference` em cada projeto confirma dependências corretas
3. **Verificação de SDK:** `dotnet --version` na raiz deve retornar 10.0.x conforme `global.json`

## Critérios de Aceite da Subtask
- [x] Solution com 6 projetos criados e adicionados
- [x] `global.json` presente e funcional (força .NET 10)
- [x] Referências entre projetos configuradas corretamente (Application não referencia Infra; Lambda não referencia Domain diretamente)
- [x] `dotnet build` na raiz compila todos os projetos com sucesso
- [x] Estrutura de pastas: `src/` e `tests/` na raiz
