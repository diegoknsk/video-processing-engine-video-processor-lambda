# Reorganização do Plano de Desenvolvimento

## 📊 Status da Reorganização

✅ **Concluída em:** 15/02/2026

## 🎯 Objetivo

Reordenar o desenvolvimento para seguir a **Alternativa 1 - Evolução Sequencial Pragmática**, priorizando funcionalidade básica antes de robustez avançada.

## 📦 Mudanças Realizadas

### 1️⃣ Stories Movidas para Backlog

As seguintes stories foram movidas para `storys/Backlog-Later/`:

- **Storie-04:** Idempotência (done.json marker) + Convenção de Prefixos S3
- **Storie-05:** Validação de Input + Classificação de Erros
- **Storie-06:** Observabilidade (Logs Estruturados + Métricas + Correlation)
- **Storie-07:** Testes BDD + Unitários + Documentação Técnica Final

**Motivo:** Estas stories focam em robustez avançada que deve ser implementada após o fluxo básico funcionar.

### 2️⃣ Novas Stories Criadas

Criadas 4 novas stories com foco pragmático:

#### **Storie-04: Deploy AWS HelloWorld + Invocação Interna**
- Lambda rodando na AWS
- Retorna "Hello World" simples
- Invocação via Console AWS e AWS CLI
- Logs no CloudWatch
- Documentação de invocação manual

**Subtasks:**
1. Simplificar handler para retornar Hello World
2. Validar deploy via GitHub Actions
3. Invocar Lambda via Console AWS e documentar
4. Invocar Lambda via AWS CLI e documentar
5. Validar logs no CloudWatch e criar guia de invocação

---

#### **Storie-05: Processamento Local de Vídeo para Frames**
- Extração de frames 100% local
- Usa FFmpeg via Xabe.FFmpeg
- Intervalo parametrizável
- Aplicação console para teste
- **NÃO usa Lambda nem S3 ainda**

**Subtasks:**
1. Instalar Xabe.FFmpeg e configurar FFmpeg localmente
2. Criar port IVideoFrameExtractor no Domain
3. Implementar VideoFrameExtractor com extração parametrizável
4. Criar aplicação console CLI para teste local
5. Validar extração com vídeo real e criar testes unitários

---

#### **Storie-06: Processamento no Lambda (/tmp) sem S3**
- Portar processamento para Lambda
- Usa diretório /tmp
- Lambda Layer com FFmpeg
- **NÃO usa S3 ainda**

**Subtasks:**
1. Criar Lambda Layer com FFmpeg binário
2. Criar FFmpegConfigurator para detectar FFmpeg no Lambda
3. Criar ProcessVideoUseCase integrando VideoFrameExtractor
4. Integrar use case no handler Lambda e processar vídeo em /tmp
5. Implementar limpeza /tmp e validar execução no Lambda

---

#### **Storie-07: Upload S3 Frames + Manifest + Log Mensageria**
- Upload de frames para S3
- Prefixos determinísticos
- Manifest.json com metadados
- Log mockado de mensageria
- **NÃO integra mensageria real ainda**

**Subtasks:**
1. Criar port IS3Service e implementação S3Service
2. Criar S3PrefixBuilder para convenção determinística
3. Implementar upload de frames para S3
4. Gerar e fazer upload de manifest.json
5. Integrar S3 no use case e validar fluxo end-to-end

---

## 🚀 Novo Fluxo de Desenvolvimento

```
✅ Storie-01: Bootstrap Projeto Lambda .NET 10
✅ Storie-02: Deploy GitHub Actions + Validação E2E
✅ Storie-03: Contrato Input/Output + Versionamento
───────────────────────────────────────────────────
🔄 Storie-04: Hello World AWS (NOVA)
   → Lambda rodando e invocável
🔄 Storie-05: Processamento Local (NOVA)
   → Algoritmo de extração validado
🔄 Storie-06: Processamento Lambda (NOVA)
   → Processamento no ambiente Lambda
🔄 Storie-07: Upload S3 (NOVA)
   → Integração completa S3
───────────────────────────────────────────────────
⏸️  Backlog-Later:
   - Idempotência forte
   - Classificação de erros
   - Observabilidade avançada
   - BDD + Cobertura ≥80%
```

## 📋 Próximos Passos

1. **Desenvolver Storie-04:** Garantir Lambda funcionando na AWS
2. **Desenvolver Storie-05:** Validar algoritmo de extração localmente
3. **Desenvolver Storie-06:** Portar para Lambda
4. **Desenvolver Storie-07:** Integrar com S3
5. **Retomar Backlog:** Após fluxo básico validado

## 📚 Documentação Criada

- ✅ `storys/Backlog-Later/README.md` - Explicação da reorganização
- ✅ Stories pausadas atualizadas com header de PAUSADA
- ✅ 4 novas stories criadas (04-07)
- ✅ 20 novas subtasks criadas (5 por story)
- ✅ Este documento de reorganização

## 🎓 Lições Aprendidas

### ✅ Boas Práticas Aplicadas
- Evolução incremental e validável
- Separação clara de responsabilidades
- Teste de cada camada antes de avançar
- Documentação acompanhando desenvolvimento

### ⚠️ Problemas Evitados
- Implementar robustez antes de funcionalidade básica
- Criar abstrações complexas prematuramente
- Focar em qualidade avançada sem ter fluxo básico
- Misturar múltiplas concerns em uma única story

## 📊 Estatísticas

- **Stories pausadas:** 4
- **Subtasks pausadas:** 22
- **Novas stories criadas:** 4
- **Novas subtasks criadas:** 20
- **Documentos criados:** 25 arquivos markdown
- **Tempo de reorganização:** ~30 minutos

---

**Reorganização realizada por:** Arquiteto de Software + Desenvolvedor .NET + DevOps Sênior  
**Data:** 15/02/2026  
**Aprovado por:** Diego (Product Owner)
