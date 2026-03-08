# Subtask 02: Garantir prioridade Layer e fallback no Function

## Descrição
Garantir que no `Function.cs` a ordem de configuração do FFmpeg priorize o Lambda Layer (`/opt/ffmpeg`, `/opt/bin`, `FFMPEG_PATH`) antes de chamar `FFmpegSetup.EnsureFFmpegInstalledAsync()`, para que na AWS, quando o Layer estiver configurado, não se tente download nem criação de diretório.

## Passos de implementação
1. Revisar `Function.cs`: o construtor já chama `TrySetFfmpegPathFromEnvOrKnownPaths()`, e o handler chama `EnsureFFmpegInstalledAsync()` apenas quando `!IsFfmpegConfigured()`. Confirmar que essa ordem está correta e que os caminhos `/opt/ffmpeg` e `/opt/bin` são verificados.
2. Se necessário, garantir que `TrySetFfmpegPathFromEnvOrKnownPaths` seja suficiente para que, com Layer anexado, `IsFfmpegConfigured()` retorne true antes de qualquer chamada a `EnsureFFmpegInstalledAsync`.
3. Opcional: no handler, antes de chamar `EnsureFFmpegInstalledAsync`, chamar novamente `TrySetFfmpegPathFromEnvOrKnownPaths()` (idempotente) para cobrir cenários em que o path só esteja disponível após o cold start; evitar lógica duplicada desnecessária se o construtor já resolver.

## Formas de teste
- Deploy do Lambda com Layer FFmpeg em `/opt/ffmpeg`; invocar e verificar nos logs que o path usado é `/opt/ffmpeg` e que não há tentativa de criar `.ffmpeg` em `/var/task`.
- Deploy sem Layer (apenas para teste): invocar e verificar que o erro não é mais "Read-only file system" (pode ser outro, ex.: timeout ou FFmpeg não encontrado, mas não IOException em /var/task).

## Critérios de aceite da subtask
- [ ] Com Layer configurado em `/opt/ffmpeg` ou `/opt/bin`, o handler usa esse path e não chama download em disco.
- [ ] Sem Layer, o handler não falha por "Read-only file system"; se falhar, é por FFmpeg não encontrado ou timeout, e a doc descreve a necessidade do Layer.
