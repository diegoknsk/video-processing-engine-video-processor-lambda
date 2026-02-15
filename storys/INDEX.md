# Índice de Stories — Lambda Video Processor (Chunk Worker)

## Visão Geral

Este documento lista todas as histórias técnicas criadas para o desenvolvimento do **Lambda Video Processor**, um worker minimalista que processa chunks individuais de vídeo no pipeline distribuído.

**Arquitetura:** Alternativa A (handler puro, sem AddAWSLambdaHosting)  
**Total de Stories:** 7  
**Tecnologias:** .NET 10, AWS Lambda, Step Functions, S3, CloudWatch

---

## Stories

### [Storie-01: Bootstrap do Projeto Lambda .NET 10 + Handler Puro + Estrutura Base](./Storie-01-Bootstrap_Projeto_Lambda_NET10/story.md)
**Objetivo:** Criar estrutura inicial do projeto Lambda com handler puro, Clean Architecture, DI, e documentação base.

**Subtasks:**
- [01: Criar estrutura de projetos e solution](./Storie-01-Bootstrap_Projeto_Lambda_NET10/subtask/Subtask-01-Criar_Estrutura_Projetos.md)
- [02: Implementar Function Handler puro e bootstrap de DI](./Storie-01-Bootstrap_Projeto_Lambda_NET10/subtask/Subtask-02-Implementar_Handler_DI.md)
- [03: Configurar empacotamento ZIP e criar README](./Storie-01-Bootstrap_Projeto_Lambda_NET10/subtask/Subtask-03-Configurar_Empacotamento_README.md)
- [04: Criar projetos de testes (Unit e BDD) com estrutura base](./Storie-01-Bootstrap_Projeto_Lambda_NET10/subtask/Subtask-04-Criar_Projetos_Testes.md)

**Status:** 🔄 Planejada  
**Dependências:** Nenhuma

---

### [Storie-02: Deploy via GitHub Actions + Validação End-to-End (AWS Academy)](./Storie-02-Deploy_GitHub_Actions_Validacao_E2E/story.md)
**Objetivo:** Configurar pipeline CI/CD com GitHub Actions usando credenciais temporárias AWS Academy, e validar deploy end-to-end com Step Functions.

**Subtasks:**
- [01: Criar workflow GitHub Actions para build e deploy](./Storie-02-Deploy_GitHub_Actions_Validacao_E2E/subtask/Subtask-01-Criar_Workflow_Deploy.md)
- [02: Implementar script de validação pós-deploy](./Storie-02-Deploy_GitHub_Actions_Validacao_E2E/subtask/Subtask-02-Script_Validacao_Pos_Deploy.md)
- [03: Criar payload de teste e documentar procedimento de validação](./Storie-02-Deploy_GitHub_Actions_Validacao_E2E/subtask/Subtask-03-Payload_Teste_Documentacao.md)
- [04: Executar deploy real e validar end-to-end](./Storie-02-Deploy_GitHub_Actions_Validacao_E2E/subtask/Subtask-04-Executar_Deploy_Validar_E2E.md)

**Status:** 🔄 Planejada  
**Dependências:** Storie-01

---

### [Storie-03: Contrato de Input/Output + Versionamento (contractVersion)](./Storie-03-Contrato_Input_Output_Versionamento/story.md)
**Objetivo:** Definir contratos tipados (records C# 13) para input/output, implementar versionamento via `contractVersion`, e validar contratos no handler.

**Subtasks:**
- [01: Criar models de input (ChunkProcessorInput e dependentes)](./Storie-03-Contrato_Input_Output_Versionamento/subtask/Subtask-01-Criar_Models_Input.md)
- [02: Criar models de output (ChunkProcessorOutput e dependentes)](./Storie-03-Contrato_Input_Output_Versionamento/subtask/Subtask-02-Criar_Models_Output.md)
- [03: Implementar versionamento e exceções de versão não suportada](./Storie-03-Contrato_Input_Output_Versionamento/subtask/Subtask-03-Implementar_Versionamento.md)
- [04: Atualizar Function handler para usar models tipados](./Storie-03-Contrato_Input_Output_Versionamento/subtask/Subtask-04-Atualizar_Handler_Models_Tipados.md)
- [05: Criar testes unitários de serialização/deserialização](./Storie-03-Contrato_Input_Output_Versionamento/subtask/Subtask-05-Testes_Serializacao.md)

**Status:** 🔄 Planejada  
**Dependências:** Storie-01

---

### [Storie-04: Idempotência (done.json marker) + Convenção de Prefixos S3](./Storie-04-Idempotencia_Done_Marker_Prefixos_S3/story.md)
**Objetivo:** Implementar idempotência usando marker `done.json`, definir convenção de prefixos S3, e escrever manifestos estruturados.

**Subtasks:**
- [01: Criar port IS3Service e implementação S3Service](./Storie-04-Idempotencia_Done_Marker_Prefixos_S3/subtask/Subtask-01-Criar_Port_S3Service.md)
- [02: Criar PrefixBuilder para convenção de prefixos determinística](./Storie-04-Idempotencia_Done_Marker_Prefixos_S3/subtask/Subtask-02-Criar_PrefixBuilder.md)
- [03: Implementar ProcessChunkUseCase com verificação de idempotência](./Storie-04-Idempotencia_Done_Marker_Prefixos_S3/subtask/Subtask-03-Implementar_UseCase_Idempotencia.md)
- [04: Escrever manifest.json e done.json mockados no S3](./Storie-04-Idempotencia_Done_Marker_Prefixos_S3/subtask/Subtask-04-Escrever_Manifest_Done_S3.md)
- [05: Criar testes unitários de idempotência e prefixos](./Storie-04-Idempotencia_Done_Marker_Prefixos_S3/subtask/Subtask-05-Testes_Idempotencia_Prefixos.md)

**Status:** 🔄 Planejada  
**Dependências:** Storie-01, Storie-03

---

### [Storie-05: Validação de Input + Classificação de Erros (retryable vs não-retryable)](./Storie-05-Validacao_Input_Classificacao_Erros/story.md)
**Objetivo:** Validar input com FluentValidation, classificar erros como retryable ou não-retryable, e tratar exceções corretamente no handler.

**Subtasks:**
- [01: Instalar FluentValidation e criar ChunkProcessorInputValidator](./Storie-05-Validacao_Input_Classificacao_Erros/subtask/Subtask-01-Criar_Validator_Input.md)
- [02: Criar exceção ChunkValidationException (não-retryable)](./Storie-05-Validacao_Input_Classificacao_Erros/subtask/Subtask-02-Criar_Exception_Validation.md)
- [03: Criar ErrorClassifier para classificar exceções](./Storie-05-Validacao_Input_Classificacao_Erros/subtask/Subtask-03-Criar_ErrorClassifier.md)
- [04: Adicionar tratamento de exceções no handler](./Storie-05-Validacao_Input_Classificacao_Erros/subtask/Subtask-04-Tratar_Excecoes_Handler.md)
- [05: Criar testes unitários de validação e classificação](./Storie-05-Validacao_Input_Classificacao_Erros/subtask/Subtask-05-Testes_Validacao_Classificacao.md)

**Status:** 🔄 Planejada  
**Dependências:** Storie-03, Storie-04

---

### [Storie-06: Observabilidade (Logs Estruturados + Métricas + Correlation)](./Storie-06-Observabilidade_Logs_Metricas_Correlation/story.md)
**Objetivo:** Implementar logs estruturados com correlation (videoId, chunkId, executionArn), métricas customizadas, e enriquecimento de logs.

**Subtasks:**
- [01: Criar CorrelationContext para propagar videoId/chunkId/executionArn](./Storie-06-Observabilidade_Logs_Metricas_Correlation/subtask/Subtask-01-Criar_CorrelationContext.md)
- [02: Configurar logging estruturado e enriquecer logs com correlation](./Storie-06-Observabilidade_Logs_Metricas_Correlation/subtask/Subtask-02-Configurar_Logging_Estruturado.md)
- [03: Adicionar logs em pontos-chave do fluxo](./Storie-06-Observabilidade_Logs_Metricas_Correlation/subtask/Subtask-03-Adicionar_Logs_Pontos_Chave.md)
- [04: Criar MetricsPublisher e publicar métricas customizadas](./Storie-06-Observabilidade_Logs_Metricas_Correlation/subtask/Subtask-04-Criar_MetricsPublisher.md)
- [05: Validar logs e métricas no CloudWatch](./Storie-06-Observabilidade_Logs_Metricas_Correlation/subtask/Subtask-05-Validar_Logs_Metricas_CloudWatch.md)

**Status:** 🔄 Planejada  
**Dependências:** Storie-04, Storie-05

---

### [Storie-07: Testes BDD + Unitários + Documentação Técnica Final](./Storie-07-Testes_BDD_Unitarios_Documentacao/story.md)
**Objetivo:** Completar suíte de testes (BDD + unitários ≥ 80% cobertura), criar ADRs, guia de troubleshooting, runbook operacional, e README final.

**Subtasks:**
- [01: Criar feature BDD end-to-end e step definitions](./Storie-07-Testes_BDD_Unitarios_Documentacao/subtask/Subtask-01-Criar_Feature_BDD_E2E.md)
- [02: Adicionar testes unitários faltantes para cobertura ≥ 80%](./Storie-07-Testes_BDD_Unitarios_Documentacao/subtask/Subtask-02-Adicionar_Testes_Unitarios_Cobertura.md)
- [03: Configurar relatórios de cobertura e workflow CI](./Storie-07-Testes_BDD_Unitarios_Documentacao/subtask/Subtask-03-Configurar_Relatorios_Cobertura.md)
- [04: Criar ADRs e documentação de convenções](./Storie-07-Testes_BDD_Unitarios_Documentacao/subtask/Subtask-04-Criar_ADRs_Convencoes.md)
- [05: Criar guia de troubleshooting e runbook operacional](./Storie-07-Testes_BDD_Unitarios_Documentacao/subtask/Subtask-05-Criar_Troubleshooting_Runbook.md)
- [06: Atualizar README final com todas as seções](./Storie-07-Testes_BDD_Unitarios_Documentacao/subtask/Subtask-06-Atualizar_README_Final.md)

**Status:** 🔄 Planejada  
**Dependências:** Todas as stories anteriores (01-06)

---

## Ordem de Execução Recomendada

```
Storie-01 (Bootstrap)
    ↓
Storie-02 (Deploy/CI/CD) ← pode rodar em paralelo com Storie-03
    ↓
Storie-03 (Contratos)
    ↓
Storie-04 (Idempotência + S3)
    ↓
Storie-05 (Validação + Erros)
    ↓
Storie-06 (Observabilidade)
    ↓
Storie-07 (Testes + Docs)
```

---

## Resumo de Entregas

### Funcional (MVP)
- ✅ Handler Lambda puro .NET 10
- ✅ Input/Output tipados com versionamento
- ✅ Idempotência (done marker)
- ✅ Manifestos mockados no S3
- ✅ Validação de input
- ✅ Classificação de erros retryable

### DevOps/CI/CD
- ✅ GitHub Actions pipeline
- ✅ Deploy automático
- ✅ Validação end-to-end
- ✅ Relatórios de cobertura

### Observabilidade
- ✅ Logs estruturados
- ✅ Correlation (videoId, chunkId, executionArn)
- ✅ Métricas customizadas (CloudWatch)

### Qualidade
- ✅ Testes BDD (SpecFlow) — requisito hackathon
- ✅ Testes unitários (≥ 80% cobertura)
- ✅ ADRs (decisões arquiteturais)
- ✅ Guia de troubleshooting
- ✅ Runbook operacional

---

## Próximos Passos

Após concluir essas 7 stories, o Lambda Video Processor estará pronto para integração no pipeline completo. Stories futuras (fora do escopo atual) podem incluir:

- **Extração real de frames** (usando FFmpeg via skill existente)
- **Upload de frames para S3**
- **Integração com serviço de metadados**
- **Otimizações de performance** (memory pooling, Span<T>)
- **Retry policies customizados** (exponential backoff)

---

**Data de Criação:** [Inserir data]  
**Arquiteto:** Diego (via Cursor Agent)  
**Versão:** 1.0
