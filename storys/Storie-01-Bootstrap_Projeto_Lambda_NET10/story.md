# Storie-01: Bootstrap do Projeto Lambda .NET 10 + Handler Puro + Estrutura Base

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do sistema de processamento de vídeo, quero criar a estrutura inicial do Lambda Worker com handler puro em .NET 10, para ter a base do componente que processará chunks individuais de vídeo no pipeline da Step Functions.

## Objetivo
Criar projeto Lambda .NET 10 minimalista (sem AddAWSLambdaHosting), com handler puro que recebe input do Map da Step Functions, estrutura de pastas seguindo Clean Architecture, configuração de DI básica, e documentação mínima para rodar localmente e empacotar.

## Escopo Técnico
- **Tecnologias:** .NET 10 SDK, C# 13, AWS Lambda Runtime
- **Arquivos criados:**
  - `src/VideoProcessor.Lambda/Function.cs` (handler puro)
  - `src/VideoProcessor.Lambda/VideoProcessor.Lambda.csproj`
  - `src/VideoProcessor.Domain/` (entidades e ports)
  - `src/VideoProcessor.Application/` (use cases)
  - `src/VideoProcessor.Infra/` (implementação S3, etc.)
  - `tests/VideoProcessor.Tests.Unit/`
  - `tests/VideoProcessor.Tests.Bdd/`
  - `README.md`
  - `.gitignore`
  - `global.json` (pinning .NET 10)
- **Componentes:** Handler Lambda, DI Container, estrutura de projetos
- **Pacotes/Dependências:**
  - Amazon.Lambda.Core (2.3.0)
  - Amazon.Lambda.Serialization.SystemTextJson (2.4.3)
  - AWSSDK.S3 (3.7.400 ou superior)
  - Microsoft.Extensions.DependencyInjection (10.0.0)
  - Microsoft.Extensions.Configuration (10.0.0)
  - Microsoft.Extensions.Configuration.EnvironmentVariables (10.0.0)

## Dependências e Riscos (para estimativa)
- **Dependências:** Nenhuma (primeira story do projeto)
- **Riscos:**
  - SDK .NET 10 pode estar em preview (mitigar: usar LTS se instável)
  - Diferenças de ambiente local vs Lambda (mitigar: testar empacotamento desde o início)
- **Pré-condições:** 
  - .NET 10 SDK instalado localmente
  - AWS CLI configurado (para testes locais com S3)

## Subtasks
- [Subtask 01: Criar estrutura de projetos e solution](./subtask/Subtask-01-Criar_Estrutura_Projetos.md)
- [Subtask 02: Implementar Function Handler puro e bootstrap de DI](./subtask/Subtask-02-Implementar_Handler_DI.md)
- [Subtask 03: Configurar empacotamento ZIP e criar README](./subtask/Subtask-03-Configurar_Empacotamento_README.md)
- [Subtask 04: Criar projetos de testes (Unit e BDD) com estrutura base](./subtask/Subtask-04-Criar_Projetos_Testes.md)

## Critérios de Aceite da História
- [ ] Solution com 5 projetos (Lambda, Domain, Application, Infra, Tests.Unit, Tests.Bdd) compilando sem erros em .NET 10
- [ ] Handler Lambda (`Function.cs`) com método `FunctionHandler` que recebe `JsonDocument` e retorna `JsonDocument` (contrato genérico inicial)
- [ ] DI configurado no construtor do handler (pattern recomendado para Lambda), registrando serviços de Application e Infra
- [ ] Comando `dotnet lambda package` gera ZIP funcional (ou `dotnet publish` + zip manual)
- [ ] README.md documenta: (a) como rodar `dotnet build`, (b) como rodar testes, (c) como empacotar ZIP, (d) estrutura de pastas
- [ ] `.gitignore` configurado para .NET (bin/, obj/, *.user, .vs/, .idea/, etc.)
- [ ] `global.json` fixa SDK em .NET 10.x
- [ ] Projeto compila e executa localmente (smoke test: handler retorna mock response)

## Rastreamento (dev tracking)
- **Início:** 14/02/2026, às 21:00 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
