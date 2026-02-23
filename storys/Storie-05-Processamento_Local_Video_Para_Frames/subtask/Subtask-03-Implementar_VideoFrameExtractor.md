# Subtask 03: Implementar VideoFrameExtractor com extração parametrizável

## Status
- [x] Concluído

## Descrição
Criar implementação concreta de `IVideoFrameExtractor` usando Xabe.FFmpeg, implementando lógica de extração de frames em intervalos parametrizáveis.

## Tarefas
1. Criar `src/VideoProcessor.Application/Services/VideoFrameExtractor.cs`
2. Implementar `IVideoFrameExtractor`:
   - Obter duração do vídeo: `await FFmpeg.GetMediaInfo(videoPath)`
   - Calcular quantidade de frames: `totalFrames = (int)(duration.TotalSeconds / intervalSeconds)`
   - Loop extraindo frames em cada timestamp: 0s, 20s, 40s, etc.
   - Nomear frames: `frame_{index:D4}_{seconds}s.jpg`
   - Usar `FFmpeg.Conversions.FromSnippet.Snapshot()` para cada frame
3. Adicionar tratamento de erros:
   - Vídeo não existe
   - Formato não suportado
   - Pasta output não existe (criar automaticamente)
4. Adicionar logs de progresso

## Critérios de Aceite
- [ ] Classe `VideoFrameExtractor` implementa `IVideoFrameExtractor`
- [ ] Extração funciona com intervalos parametrizáveis (5s, 10s, 20s, etc.)
- [ ] Frames nomeados determinísticamente: `frame_0001_0s.jpg`, `frame_0002_20s.jpg`
- [ ] Pasta de output criada automaticamente se não existir
- [ ] Tratamento de erros para casos comuns
- [ ] Log mostra progresso: "Extraindo frame 10 de 50..."
- [ ] Retorna `FrameExtractionResult` com todos os campos preenchidos

## Notas Técnicas
- Usar `Directory.CreateDirectory(outputFolder)` (não falha se já existe)
- TimeSpan para timestamp: `TimeSpan.FromSeconds(i * intervalSeconds)`
- Processar frames sequencialmente para evitar excesso de CPU
- Formato JPEG padrão (qualidade 85-90)
