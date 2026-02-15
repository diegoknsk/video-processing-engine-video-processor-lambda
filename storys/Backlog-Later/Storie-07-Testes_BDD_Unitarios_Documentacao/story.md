# Storie-07: Testes BDD + Unitários + Documentação Técnica Final

> ⚠️ **STATUS: PAUSADA** – Esta story foi movida para o backlog. Executar após:
> - Processamento real de vídeo implementado (Storie-05 nova)
> - Processamento rodando no Lambda (Storie-06 nova)
> - Integração S3 básica funcionando (Storie-07 nova)
> 
> Motivo: BDD end-to-end e cobertura ≥80% devem ser implementados após o fluxo básico funcionar.

## Status
- **Estado:** ⏸️ Pausada
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do projeto, quero completar a suíte de testes incluindo ao menos 1 teste BDD end-to-end (requisito do hackathon), aumentar cobertura de testes unitários para ≥ 80%, e criar documentação técnica final (ADRs, convenções, troubleshooting), para garantir qualidade, testabilidade e manutenibilidade do Lambda Worker.

## Objetivo
Criar feature BDD que valida fluxo completo (input → validação → idempotência → S3 → output), adicionar testes unitários faltantes para atingir cobertura ≥ 80%, configurar relatórios de cobertura, e criar documentação técnica incluindo decisões arquiteturais (ADRs), convenções S3, guia de troubleshooting e runbook operacional.

## Escopo Técnico
- **Tecnologias:** SpecFlow, xUnit, coverlet, ReportGenerator, Markdown
- **Arquivos criados/modificados:**
  - `tests/VideoProcessor.Tests.Bdd/Features/ProcessChunk.feature`
  - `tests/VideoProcessor.Tests.Bdd/StepDefinitions/ProcessChunkSteps.cs`
  - `tests/VideoProcessor.Tests.Bdd/Hooks/TestSetup.cs`
  - Testes unitários adicionais para atingir cobertura
  - `.github/workflows/test-coverage.yml` (workflow para cobertura)
  - `docs/architecture/ADR-001-handler-puro-sem-api-hosting.md`
  - `docs/architecture/ADR-002-idempotencia-done-marker.md`
  - `docs/architecture/ADR-003-classificacao-erros-retryable.md`
  - `docs/S3_CONVENTIONS.md` (já criado na Storie-04, expandir)
  - `docs/TROUBLESHOOTING.md`
  - `docs/RUNBOOK.md`
  - `README.md` (atualização final com todas as seções)
- **Componentes:** Testes BDD, testes unitários, relatórios de cobertura, documentação
- **Pacotes/Dependências:**
  - SpecFlow.xUnit (3.9.74) — já instalado na Storie-01
  - ReportGenerator (5.2.0 ou superior) — para relatórios HTML de cobertura

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Todas as stories anteriores concluídas (01-06)
- **Riscos:**
  - Cobertura < 80% pode exigir refatoração de código para melhorar testabilidade (mitigar: identificar gaps cedo)
- **Pré-condições:**
  - Todos os componentes implementados e funcionais

## Subtasks
- [Subtask 01: Criar feature BDD end-to-end e step definitions](./subtask/Subtask-01-Criar_Feature_BDD_E2E.md)
- [Subtask 02: Adicionar testes unitários faltantes para cobertura ≥ 80%](./subtask/Subtask-02-Adicionar_Testes_Unitarios_Cobertura.md)
- [Subtask 03: Configurar relatórios de cobertura e workflow CI](./subtask/Subtask-03-Configurar_Relatorios_Cobertura.md)
- [Subtask 04: Criar ADRs e documentação de convenções](./subtask/Subtask-04-Criar_ADRs_Convencoes.md)
- [Subtask 05: Criar guia de troubleshooting e runbook operacional](./subtask/Subtask-05-Criar_Troubleshooting_Runbook.md)
- [Subtask 06: Atualizar README final com todas as seções](./subtask/Subtask-06-Atualizar_README_Final.md)

## Critérios de Aceite da História
- [ ] Feature BDD `ProcessChunk.feature` criada com cenários: chunk novo (sucesso), chunk já processado (idempotente), input inválido (falha)
- [ ] Step definitions implementados com mocks de S3Service
- [ ] Teste BDD executa e passa: `dotnet test tests/VideoProcessor.Tests.Bdd`
- [ ] Cobertura de código ≥ 80% em todos os projetos (exceto Lambda bootstrap e testes)
- [ ] Relatório de cobertura HTML gerado via ReportGenerator
- [ ] Workflow GitHub Actions gera e publica relatório de cobertura
- [ ] ADRs criados documentando: (a) handler puro sem API hosting, (b) idempotência com done marker, (c) classificação de erros retryable
- [ ] `docs/S3_CONVENTIONS.md` documenta estrutura de prefixos e chaves
- [ ] `docs/TROUBLESHOOTING.md` lista problemas comuns e soluções (ex.: credenciais expiradas, done marker órfão, logs não aparecem)
- [ ] `docs/RUNBOOK.md` documenta: como monitorar Lambda, como investigar falhas, como forçar reprocessamento
- [ ] `README.md` atualizado com: visão geral, arquitetura, pré-requisitos, como buildar/testar/deployar, estrutura do projeto, observabilidade, troubleshooting, links para docs

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
