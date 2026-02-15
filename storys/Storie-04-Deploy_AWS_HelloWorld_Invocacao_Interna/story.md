# Storie-04: Deploy AWS HelloWorld + Invocação Interna

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do Lambda Worker, quero garantir que o Lambda está rodando na AWS e pode ser invocado internamente (Console AWS, AWS CLI, Step Functions), para validar que a infraestrutura básica funciona antes de implementar processamento real de vídeo.

## Objetivo
Atualizar o handler Lambda para retornar um payload "Hello World" simples mas informativo, garantir que o pipeline GitHub Actions faz deploy correto, testar invocação manual via Console AWS e AWS CLI, documentar logs no CloudWatch, e criar instruções claras no README de como invocar manualmente o Lambda.

## Escopo Técnico
- **Tecnologias:** AWS Lambda, .NET 10, C# 13, AWS CLI, CloudWatch Logs
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Lambda/Function.cs` (simplificar para Hello World)
  - `tests/payloads/hello-world-test.json` (payload de teste para AWS)
  - `README.md` (adicionar seção "Como Invocar Lambda Manualmente")
  - `docs/INVOCATION_GUIDE.md` (guia detalhado de invocação)
  - `.github/workflows/deploy.yml` (validar deploy automático)
- **Componentes:** Lambda handler simplificado, logs CloudWatch, documentação de invocação
- **Pacotes/Dependências:**
  - Nenhum pacote novo necessário (usar apenas Amazon.Lambda.Core já instalado)

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-01 concluída (estrutura base)
  - Storie-02 concluída (pipeline GitHub Actions)
- **Riscos:**
  - Credenciais AWS expiradas podem impedir invocação manual (mitigar: documentar troubleshooting)
  - Logs podem não aparecer imediatamente no CloudWatch (mitigar: documentar delay esperado)
- **Pré-condições:**
  - Lambda criado na AWS
  - Credenciais AWS configuradas localmente
  - Pipeline GitHub Actions funcional

## Subtasks
- [Subtask 01: Simplificar handler para retornar Hello World](./subtask/Subtask-01-Simplificar_Handler_HelloWorld.md)
- [Subtask 02: Validar deploy via GitHub Actions](./subtask/Subtask-02-Validar_Deploy_GitHub_Actions.md)
- [Subtask 03: Invocar Lambda via Console AWS e documentar](./subtask/Subtask-03-Invocar_Console_AWS.md)
- [Subtask 04: Invocar Lambda via AWS CLI e documentar](./subtask/Subtask-04-Invocar_AWS_CLI.md)
- [Subtask 05: Validar logs no CloudWatch e criar guia de invocação](./subtask/Subtask-05-Validar_Logs_CloudWatch.md)

## Critérios de Aceite da História
- [ ] `Function.cs` retorna payload JSON: `{ "message": "Hello World from Video Processor Lambda", "version": "1.0.0", "timestamp": "2026-02-15T17:00:00Z", "environment": "dev" }`
- [ ] Pipeline GitHub Actions executa com sucesso e atualiza função Lambda
- [ ] Lambda pode ser invocado manualmente via Console AWS (seção "Test")
- [ ] Lambda pode ser invocado via AWS CLI: `aws lambda invoke --function-name video-processor-lambda --payload '{}' response.json`
- [ ] Arquivo `tests/payloads/hello-world-test.json` criado com payload de exemplo para teste manual no Console AWS
- [ ] Logs aparecem no CloudWatch Logs com: timestamp, request ID, mensagem "Hello World invoked"
- [ ] `README.md` atualizado com seção "Como Invocar Lambda Manualmente" (Console + CLI)
- [ ] `docs/INVOCATION_GUIDE.md` criado com: screenshots do Console, comandos CLI completos, troubleshooting comum, e referência ao payload de teste
- [ ] Teste manual executado e documentado com prints/screenshots

## Rastreamento (dev tracking)
- **Início:** dia 15/02/2026, às 17:14 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
