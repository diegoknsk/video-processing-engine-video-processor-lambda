# Backlog - Stories Pausadas

## Motivo da Reorganização

Durante o desenvolvimento, identificamos que as stories 04-07 originais estavam muito avançadas para o estágio atual do projeto. Elas focavam em robustez e qualidade avançadas (idempotência, classificação de erros, observabilidade, BDD) antes mesmo de termos o processamento básico funcionando.

## Estratégia Adotada

Optamos pela **Alternativa 1 - Evolução Sequencial Pragmática**, que prioriza:

1. ✅ Lambda funcionando na AWS com "Hello World"
2. ✅ Processamento real de vídeo local (fora da AWS)
3. ✅ Portar processamento para o Lambda (sem S3 inicialmente)
4. ✅ Integrar com S3 (upload de frames + manifest)
5. 🔄 **Depois**: Idempotência, classificação de erros, observabilidade avançada, BDD

## Stories Pausadas

Estas stories foram movidas para este backlog e serão implementadas **após** o fluxo básico estar funcionando:

### Storie-04: Idempotência (done.json marker) + Convenção de Prefixos S3
- **Motivo:** Idempotência é robustez avançada
- **Quando implementar:** Após Storie-07 nova (upload S3 funcionando)
- **Valor:** Evita reprocessamento em retries da Step Functions

### Storie-05: Validação de Input + Classificação de Erros
- **Motivo:** Classificação formal de erros é refinamento avançado
- **Quando implementar:** Após fluxo básico validado em produção
- **Valor:** Distingue erros retryable vs não-retryable para otimizar retries

### Storie-06: Observabilidade (Logs Estruturados + Métricas + Correlation)
- **Motivo:** Observabilidade avançada com métricas customizadas
- **Quando implementar:** Após primeiras execuções reais
- **Valor:** Facilita troubleshooting e monitoramento em produção

### Storie-07: Testes BDD + Unitários + Documentação Técnica Final
- **Motivo:** BDD e cobertura ≥80% são requisitos de qualidade final
- **Quando implementar:** Quando todas as features estiverem estáveis
- **Valor:** Garante qualidade e manutenibilidade do código

## Novas Stories 04-07

As novas stories focam no essencial:

- **Storie-04:** Deploy AWS + HelloWorld + Invocação Interna
- **Storie-05:** Processamento Local de Vídeo para Frames
- **Storie-06:** Processamento no Lambda (/tmp) sem S3
- **Storie-07:** Upload S3 Frames + Manifest + Log Mensageria

## Quando Retomar Este Backlog

Após concluir as novas stories 04-07, revisitar este backlog seguindo a ordem:
1. Idempotência (fundamental para produção)
2. Validação + Classificação de Erros (melhora qualidade)
3. Observabilidade (facilita operação)
4. BDD + Documentação (finalização do projeto)

---

**Data da reorganização:** 15/02/2026  
**Responsável:** Arquiteto de Software + DevOps Sênior
