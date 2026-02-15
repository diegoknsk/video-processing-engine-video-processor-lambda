# Subtask 03: Criar ProcessVideoUseCase integrando VideoFrameExtractor

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Criar use case `ProcessVideoUseCase` que orquestra o processamento de vídeo: inicializa FFmpeg, chama `VideoFrameExtractor`, coleta métricas, e retorna resultado estruturado.

## Tarefas
1. Criar `src/VideoProcessor.Application/UseCases/ProcessVideoUseCase.cs`
2. Implementar:
   - Receber: `videoPath` (local), `intervalSeconds`, `outputFolder`
   - Chamar `FFmpegConfigurator.InitializeFFmpeg()` (se não inicializado)
   - Chamar `videoFrameExtractor.ExtractFramesAsync()`
   - Retornar: `VideoProcessingResult` com framesCount, duration, framePaths
3. Criar model `VideoProcessingResult`:
   ```csharp
   public record VideoProcessingResult(
       string Status,
       int FramesCount,
       TimeSpan ProcessingDuration,
       List<string> FramePaths,
       string Message);
   ```
4. Adicionar logs estruturados em pontos-chave
5. Criar testes unitários mockando `IVideoFrameExtractor`

## Critérios de Aceite
- [ ] Use case `ProcessVideoUseCase` criado
- [ ] Recebe parâmetros: videoPath, intervalSeconds, outputFolder
- [ ] Inicializa FFmpeg automaticamente
- [ ] Chama `VideoFrameExtractor` e aguarda conclusão
- [ ] Retorna `VideoProcessingResult` com todos os campos
- [ ] Logs estruturados: início, progresso, fim
- [ ] Testes unitários cobrem: processamento com sucesso, falha de FFmpeg

## Notas Técnicas
- Use case não deve conhecer Lambda (independente de infraestrutura)
- Inicialização FFmpeg pode ser estática (lazy initialization)
- Capturar duração usando `Stopwatch`
- Status possíveis: "SUCCEEDED", "FAILED"
