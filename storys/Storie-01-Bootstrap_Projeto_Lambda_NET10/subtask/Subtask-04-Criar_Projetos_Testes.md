# Subtask 04: Criar Projetos de Testes (Unit e BDD) com Estrutura Base

## Descrição
Configurar projetos de testes unitários (xUnit) e BDD (SpecFlow + xUnit), instalar pacotes necessários, criar estrutura de pastas base e um teste smoke para validar que infraestrutura de testes funciona.

## Passos de Implementação
1. No projeto `VideoProcessor.Tests.Unit`, instalar pacotes:
   ```bash
   dotnet add package xunit --version 2.9.0
   dotnet add package xunit.runner.visualstudio --version 2.8.2
   dotnet add package FluentAssertions --version 7.0.0
   dotnet add package Moq --version 4.20.0
   dotnet add package coverlet.collector --version 6.0.0
   ```
2. No projeto `VideoProcessor.Tests.Bdd`, instalar pacotes:
   ```bash
   dotnet add package SpecFlow.xUnit --version 3.9.74
   dotnet add package SpecFlow.Tools.MsBuild.Generation --version 3.9.74
   dotnet add package FluentAssertions --version 7.0.0
   ```
3. Criar estrutura de pastas em `Tests.Unit`:
   - `Domain/`
   - `Application/`
   - `Infra/`
4. Criar estrutura em `Tests.Bdd`:
   - `Features/` (para arquivos .feature)
   - `StepDefinitions/` (para step definitions)
   - `Hooks/` (para setup/teardown)
5. Criar teste smoke em `Tests.Unit`: `SmokeTest.cs` com teste trivial que sempre passa (ex.: `Assert.True(true)`)
6. Criar feature smoke em `Tests.Bdd`: `Smoke.feature` com cenário trivial:
   ```gherkin
   Feature: Smoke Test
   Scenario: Projeto BDD está configurado
     Given o projeto de testes BDD existe
     When eu executo os testes
     Then o teste deve passar
   ```
7. Criar step definition correspondente que sempre passa

## Formas de Teste
1. **Testes unitários:** `dotnet test tests/VideoProcessor.Tests.Unit` executa e passa smoke test
2. **Testes BDD:** `dotnet test tests/VideoProcessor.Tests.Bdd` executa e passa smoke feature
3. **Cobertura:** `dotnet test --collect:"XPlat Code Coverage"` gera relatório (mesmo que mínimo)

## Critérios de Aceite da Subtask
- [ ] Projeto `Tests.Unit` configurado com xUnit, FluentAssertions, Moq e coverlet
- [ ] Projeto `Tests.Bdd` configurado com SpecFlow.xUnit
- [ ] Estrutura de pastas criada em ambos os projetos
- [ ] Smoke test unitário passa: `dotnet test tests/VideoProcessor.Tests.Unit` retorna sucesso
- [ ] Smoke feature BDD passa: `dotnet test tests/VideoProcessor.Tests.Bdd` retorna sucesso
- [ ] `dotnet test` na raiz executa todos os testes (Unit + BDD) e reporta sucesso
