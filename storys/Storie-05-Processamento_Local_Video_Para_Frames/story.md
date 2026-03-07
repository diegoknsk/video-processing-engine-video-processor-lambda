# Storie-05: Processamento Local de Vídeo para Frames

## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 22/02/2026

## Descrição
Como desenvolvedor do Lambda Worker, quero criar a lógica real de processamento de vídeo que extrai frames em intervalos parametrizáveis e salva em pasta local, para validar que o algoritmo de extração funciona corretamente antes de portá-lo para o ambiente Lambda.

## Objetivo
Instalar Xabe.FFmpeg, configurar FFmpeg localmente (Windows), criar serviço `VideoFrameExtractor` que recebe caminho de vídeo e intervalo, extrai frames ordenados salvando em pasta local, implementar aplicação console para teste, e garantir que o processo é determinístico e eficiente.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, FFmpeg (via Xabe.FFmpeg), System.IO
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Domain/Services/IVideoFrameExtractor.cs` (port)
  - `src/VideoProcessor.Application/Services/VideoFrameExtractor.cs` (implementação)
  - `src/VideoProcessor.CLI/Program.cs` (aplicação console para teste local)
  - `src/VideoProcessor.CLI/VideoProcessor.CLI.csproj` (novo projeto)
  - `tests/VideoProcessor.Tests.Unit/Application/Services/VideoFrameExtractorTests.cs`
  - `README.md` (adicionar seção "Processamento Local de Vídeo")
- **Componentes:** VideoFrameExtractor, FFmpeg wrapper, aplicação console de teste
- **Pacotes/Dependências:**
  - Xabe.FFmpeg (5.2.6)
  - FFmpeg binário (download manual ou via Xabe.FFmpeg.Downloader)

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-01 concluída (estrutura base)
  - FFmpeg disponível localmente
- **Riscos:**
  - FFmpeg pode não estar instalado no ambiente (mitigar: usar Xabe.FFmpeg.Downloader)
  - Vídeos grandes podem exceder memória (mitigar: processar streaming, não carregar tudo em memória)
  - Formato de vídeo não suportado (mitigar: validar formato antes de processar)
- **Pré-condições:**
  - Vídeo de teste disponível (ex: sample.mp4)
  - Espaço em disco para frames extraídos

## Subtasks
- [x] [Subtask 01: Instalar Xabe.FFmpeg e configurar FFmpeg localmente](./subtask/Subtask-01-Instalar_Xabe_FFmpeg.md)
- [x] [Subtask 02: Criar port IVideoFrameExtractor no Domain](./subtask/Subtask-02-Criar_Port_VideoFrameExtractor.md)
- [x] [Subtask 03: Implementar VideoFrameExtractor com extração parametrizável](./subtask/Subtask-03-Implementar_VideoFrameExtractor.md)
- [x] [Subtask 04: Criar aplicação console CLI para teste local](./subtask/Subtask-04-Criar_CLI_Teste_Local.md)
- [x] [Subtask 05: Validar extração com vídeo real e criar testes unitários](./subtask/Subtask-05-Validar_Extracao_Testes.md)

## Critérios de Aceite da História
- [x] Xabe.FFmpeg instalado (versão 5.2.6)
- [x] FFmpeg configurado e funcional localmente (Windows)
- [x] Port `IVideoFrameExtractor` criado com métodos: `Task<FrameExtractionResult> ExtractFramesAsync(string videoPath, int intervalSeconds, string outputFolder)`
- [x] `VideoFrameExtractor` implementado usando Xabe.FFmpeg
- [x] Extração gera frames ordenados: `frame_0001_0s.jpg`, `frame_0002_20s.jpg`, etc.
- [x] Intervalo parametrizável (ex: 20s entre frames)
- [x] Aplicação console CLI criada: `dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output output/frames`
- [x] Execução local processa vídeo e gera frames em pasta `output/frames/`
- [x] Log mostra progresso: "Extraindo frames... 10/50 concluídos"
- [x] Contagem de frames determinística (mesmo vídeo + mesmo intervalo = mesma quantidade)
- [x] Testes unitários cobrem: cálculo de frames esperados, validação de parâmetros, sucesso de extração
- [x] `README.md` documenta como executar processamento local

## Rastreamento (dev tracking)
- **Início:** 22/02/2026, às 20:28 (Brasília)
- **Fim:** 22/02/2026, às 21:01 (Brasília)
- **Tempo total de desenvolvimento:** 33min
