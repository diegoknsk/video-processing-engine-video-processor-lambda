# Subtask 02: Criar FFmpegConfigurator para detectar FFmpeg no Lambda

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Criar serviço `FFmpegConfigurator` que detecta automaticamente o caminho do FFmpeg em diferentes ambientes (local vs Lambda), testando múltiplos paths possíveis.

## Tarefas
1. Criar `src/VideoProcessor.Application/Services/FFmpegConfigurator.cs`
2. Implementar método `InitializeFFmpeg()`:
   - Testar paths: `/opt/ffmpeg/`, `/opt/bin/`, `/var/task/`, local (Windows)
   - Validar que ambos `ffmpeg` e `ffprobe` existem
   - Configurar Xabe.FFmpeg: `FFmpeg.SetExecutablesPath(ffmpegDir)`
   - Logar path detectado
   - Lançar exceção se FFmpeg não encontrado
3. Adicionar log estruturado mostrando path usado
4. Criar testes unitários mockando filesystem

## Critérios de Aceite
- [ ] Classe `FFmpegConfigurator` criada
- [ ] Método `InitializeFFmpeg()` testa múltiplos paths
- [ ] Configuração funciona local (Windows) e Lambda (/opt/ffmpeg/)
- [ ] Log mostra: "FFmpeg configurado em: /opt/ffmpeg"
- [ ] Exceção clara se FFmpeg não encontrado: "FFmpeg não encontrado. Verifique Lambda Layer."
- [ ] Testes unitários cobrem: FFmpeg encontrado, FFmpeg não encontrado

## Notas Técnicas
- Ordem de prioridade: `/opt/ffmpeg/` (Layer), `/opt/bin/`, local
- Usar `File.Exists()` para validar binários
- Executar `InitializeFFmpeg()` no início do handler Lambda
- Cache estático: executar apenas uma vez por container Lambda
