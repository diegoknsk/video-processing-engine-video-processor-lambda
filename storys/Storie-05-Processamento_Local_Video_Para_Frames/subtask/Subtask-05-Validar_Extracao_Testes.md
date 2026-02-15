# Subtask 05: Validar extração com vídeo real e criar testes unitários

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Executar processamento local com vídeo real, validar que frames são gerados corretamente, e criar testes unitários para `VideoFrameExtractor` cobrindo cenários principais.

## Tarefas
1. Obter vídeo de teste (ex: sample.mp4 ~1-2min, formato comum MP4/H264)
2. Executar CLI:
   ```bash
   dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output output/frames
   ```
3. Validar resultado:
   - Frames gerados em `output/frames/`
   - Contagem correta (duração / intervalo)
   - Nomes de arquivos ordenados
   - Imagens visualizáveis (abrir para verificar)
4. Criar testes unitários em `tests/VideoProcessor.Tests.Unit/Application/Services/VideoFrameExtractorTests.cs`:
   - `ExtractFrames_ValidVideo_ReturnsCorrectFrameCount`
   - `ExtractFrames_InvalidVideoPath_ThrowsException`
   - `ExtractFrames_OutputFolderNotExists_CreatesFolder`
5. Documentar processo no README

## Critérios de Aceite
- [ ] Processamento local executado com sucesso
- [ ] Frames gerados e visualizáveis (JPEG válidos)
- [ ] Contagem de frames determinística (mesma entrada = mesma saída)
- [ ] Nomes de arquivos ordenados lexicograficamente
- [ ] Testes unitários criados cobrindo cenários principais
- [ ] Todos os testes passam: `dotnet test tests/VideoProcessor.Tests.Unit`
- [ ] `README.md` atualizado com seção "Processamento Local de Vídeo" explicando como usar CLI

## Notas Técnicas
- Para testes: usar vídeo pequeno (10-30s) para rapidez
- Testes unitários podem mockar FFmpeg ou usar vídeo real embarcado como resource
- Validar que frames têm tamanho > 0 bytes
- Verificar que primeira frame é timestamp 0s
