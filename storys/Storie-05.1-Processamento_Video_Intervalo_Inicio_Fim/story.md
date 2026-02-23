# Storie-05.1: Processamento de Vídeo com Intervalo de Tempo (Início/Fim)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do Lambda Worker, quero que o core de processamento de vídeo aceite opcionalmente o tempo de início e o tempo de fim do trecho a processar, para poder dividir um vídeo longo em vários processos independentes (ex.: vídeo de 10 minutos em processo 1 de 0s a 59s, processo 2 de 60s a 119s, etc.).

## Objetivo
Estender a interface e a implementação do extrator de frames com parâmetros opcionais de tempo de início e fim; quando informados, processar apenas o trecho [início, fim] do vídeo; quando omitidos, manter o comportamento atual (vídeo inteiro). Permitir execução da CLI e do uso futuro no Lambda com esses parâmetros opcionais.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, FFmpeg (via Xabe.FFmpeg)
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Domain/Services/IVideoFrameExtractor.cs` (adicionar parâmetros opcionais)
  - `src/VideoProcessor.Application/Services/VideoFrameExtractor.cs` (lógica de intervalo)
  - `src/VideoProcessor.CLI/Program.cs` (argumentos `--start` e `--end` opcionais)
  - `tests/VideoProcessor.Tests.Unit/Application/Services/VideoFrameExtractorTests.cs` (novos cenários)
- **Componentes:** IVideoFrameExtractor, VideoFrameExtractor, CLI
- **Pacotes/Dependências:** Nenhum novo (Xabe.FFmpeg já utilizado na Storie-05)

## Dependências e Riscos (para estimativa)
- **Dependências:** Storie-05 concluída (processamento local de vídeo para frames)
- **Riscos:**
  - start >= end ou end > duração do vídeo (mitigar: validação e mensagens claras)
  - Incompatibilidade com chamadores existentes (mitigar: parâmetros opcionais, overload ou parâmetros nullable)
- **Pré-condições:** Vídeo de teste disponível; comportamento atual (sem início/fim) preservado

## Subtasks
- [ ] [Subtask 01: Estender port IVideoFrameExtractor com parâmetros opcionais de início e fim](./subtask/Subtask-01-Estender_Port_Inicio_Fim.md)
- [ ] [Subtask 02: Implementar processamento por intervalo no VideoFrameExtractor](./subtask/Subtask-02-Implementar_Intervalo_Extractor.md)
- [ ] [Subtask 03: Atualizar CLI com argumentos --start e --end opcionais](./subtask/Subtask-03-CLI_Argumentos_Start_End.md)
- [ ] [Subtask 04: Testes unitários e validação do intervalo](./subtask/Subtask-04-Testes_Intervalo_Inicio_Fim.md)

## Critérios de Aceite da História
- [ ] Port `IVideoFrameExtractor` aceita parâmetros opcionais de tempo de início e fim (ex.: `int? startTimeSeconds`, `int? endTimeSeconds` ou overload).
- [ ] Quando início e fim são omitidos, o comportamento é idêntico ao atual (processar vídeo inteiro).
- [ ] Quando início e/ou fim são informados, apenas o trecho [início, fim] é processado (ex.: start=0, end=59 processa do 0s ao 59s).
- [ ] Validação: start < end; end não pode exceder a duração do vídeo; start >= 0.
- [ ] CLI aceita `--start N` e `--end N` (segundos) opcionais; exemplo: `--video sample.mp4 --interval 20 --output out/ --start 0 --end 59`.
- [ ] Frames gerados em modo intervalo mantêm nomenclatura consistente (ex.: frame_0001_0s.jpg no trecho 0–59).
- [ ] Testes unitários cobrem: intervalo válido, start >= end, end > duração, parâmetros omitidos (backward compatibility).

## Rastreamento (dev tracking)
- **Início:** 22/02/2026, às 21:17 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
