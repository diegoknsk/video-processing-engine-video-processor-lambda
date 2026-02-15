# Storie-06: Processamento no Lambda (/tmp) sem S3

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do Lambda Worker, quero portar a lógica de processamento de vídeo para executar dentro do Lambda utilizando diretório /tmp como armazenamento temporário, para validar que o processamento funciona no ambiente Lambda antes de integrar com S3.

## Objetivo
Criar Lambda Layer contendo FFmpeg binário, adaptar `VideoFrameExtractor` para funcionar no ambiente Lambda (configurando caminho FFmpeg para /opt/ffmpeg), receber vídeo simulado ou base64 no input, processar e gerar frames em /tmp, retornar contagem de frames e logs, e implementar limpeza de /tmp ao final.

## Escopo Técnico
- **Tecnologias:** AWS Lambda, Lambda Layers, FFmpeg, .NET 10, /tmp filesystem
- **Arquivos criados/modificados:**
  - `lambda-layers/ffmpeg/ffmpeg` (binário FFmpeg para Lambda Layer)
  - `lambda-layers/ffmpeg/ffprobe` (binário FFprobe para Lambda Layer)
  - `src/VideoProcessor.Application/Services/FFmpegConfigurator.cs` (detectar FFmpeg no Lambda)
  - `src/VideoProcessor.Lambda/Function.cs` (integrar processamento)
  - `src/VideoProcessor.Application/UseCases/ProcessVideoUseCase.cs` (novo use case)
  - `tests/VideoProcessor.Tests.Unit/Application/UseCases/ProcessVideoUseCaseTests.cs`
  - `docs/LAMBDA_LAYER_FFMPEG.md` (guia de criação do Layer)
- **Componentes:** Lambda Layer, FFmpegConfigurator, ProcessVideoUseCase, limpeza /tmp
- **Pacotes/Dependências:**
  - Nenhum pacote novo (usar Xabe.FFmpeg já instalado)

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-05 concluída (VideoFrameExtractor funcionando local)
  - Lambda Layer com FFmpeg criado
- **Riscos:**
  - FFmpeg Layer mal configurado pode causar "file not found" (mitigar: testar paths)
  - /tmp limitado a 512MB-10GB dependendo config (mitigar: processar chunks pequenos, limpar ao final)
  - Timeout Lambda pode interromper processamento (mitigar: aumentar timeout para 300s+)
  - Memória insuficiente para vídeos grandes (mitigar: aumentar memória Lambda para 2048MB+)
- **Pré-condições:**
  - Lambda Layer com FFmpeg publicado na AWS
  - Lambda configurado com Layer anexado

## Subtasks
- [Subtask 01: Criar Lambda Layer com FFmpeg binário](./subtask/Subtask-01-Criar_Lambda_Layer_FFmpeg.md)
- [Subtask 02: Criar FFmpegConfigurator para detectar FFmpeg no Lambda](./subtask/Subtask-02-Criar_FFmpegConfigurator.md)
- [Subtask 03: Criar ProcessVideoUseCase integrando VideoFrameExtractor](./subtask/Subtask-03-Criar_ProcessVideoUseCase.md)
- [Subtask 04: Integrar use case no handler Lambda e processar vídeo em /tmp](./subtask/Subtask-04-Integrar_UseCase_Lambda.md)
- [Subtask 05: Implementar limpeza /tmp e validar execução no Lambda](./subtask/Subtask-05-Implementar_Limpeza_Validar_Lambda.md)

## Critérios de Aceite da História
- [ ] Lambda Layer criado contendo FFmpeg e FFprobe em `/opt/ffmpeg/`
- [ ] Layer publicado na AWS e anexado à função Lambda
- [ ] `FFmpegConfigurator` detecta FFmpeg em múltiplos paths: `/opt/ffmpeg/`, `/opt/bin/`, `/var/task/`
- [ ] `ProcessVideoUseCase` criado recebendo: videoPath (local em /tmp), intervalSeconds, outputFolder
- [ ] Handler Lambda:
  - Recebe input: `{ "videoBase64": "...", "intervalSeconds": 20 }`
  - Salva vídeo em `/tmp/input.mp4`
  - Chama `ProcessVideoUseCase`
  - Retorna: `{ "status": "SUCCEEDED", "framesCount": 17, "processingDuration": "8.5s", "message": "Frames gerados: 17" }`
- [ ] Frames gerados em `/tmp/frames/`
- [ ] Log CloudWatch mostra: "Frames gerados: X, Duração: Y segundos"
- [ ] Limpeza de /tmp ao final (remover vídeo + frames)
- [ ] Falha controlada se limites excedidos (memória, timeout, /tmp space)
- [ ] Execução testável via invocação manual no Console AWS
- [ ] Testes unitários cobrem: use case com sucesso, falha de FFmpeg, limpeza /tmp
- [ ] `docs/LAMBDA_LAYER_FFMPEG.md` documenta como criar e anexar Layer

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
