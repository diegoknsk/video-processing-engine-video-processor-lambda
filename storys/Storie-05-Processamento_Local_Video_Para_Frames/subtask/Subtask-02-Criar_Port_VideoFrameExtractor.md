# Subtask 02: Criar port IVideoFrameExtractor no Domain

## Status
- [x] Concluído

## Descrição
Criar interface (port) `IVideoFrameExtractor` no Domain seguindo Clean Architecture, definindo contrato para extração de frames de vídeos.

## Tarefas
1. Criar arquivo `src/VideoProcessor.Domain/Services/IVideoFrameExtractor.cs`
2. Definir interface:
   ```csharp
   public interface IVideoFrameExtractor
   {
       Task<FrameExtractionResult> ExtractFramesAsync(
           string videoPath, 
           int intervalSeconds, 
           string outputFolder);
   }
   ```
3. Criar model `FrameExtractionResult` em `src/VideoProcessor.Domain/Models/FrameExtractionResult.cs`:
   ```csharp
   public record FrameExtractionResult(
       int TotalFrames,
       List<string> FramePaths,
       TimeSpan VideoDuration,
       TimeSpan ProcessingDuration);
   ```
4. Adicionar XML documentation comments

## Critérios de Aceite
- [ ] Interface `IVideoFrameExtractor` criada em `Domain/Services`
- [ ] Método `ExtractFramesAsync` definido com assinatura correta
- [ ] Model `FrameExtractionResult` criado com propriedades: TotalFrames, FramePaths, VideoDuration, ProcessingDuration
- [ ] XML comments documentam parâmetros e retorno
- [ ] Código compila sem erros

## Notas Técnicas
- Domain não deve ter dependência de FFmpeg (só interface)
- `FramePaths` contém caminhos completos dos frames gerados
- `ProcessingDuration` útil para métricas futuras
