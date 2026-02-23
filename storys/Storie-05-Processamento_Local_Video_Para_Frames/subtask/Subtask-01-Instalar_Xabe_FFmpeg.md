# Subtask 01: Instalar Xabe.FFmpeg e configurar FFmpeg localmente

## Status
- [x] Concluído

## Descrição
Instalar pacote NuGet Xabe.FFmpeg no projeto Application, baixar binários FFmpeg para ambiente Windows local, e validar que FFmpeg está funcional.

## Tarefas
1. Adicionar pacote Xabe.FFmpeg ao projeto Application:
   ```bash
   dotnet add src/VideoProcessor.Application/VideoProcessor.Application.csproj package Xabe.FFmpeg --version 5.2.6
   ```
2. Criar método helper para download de FFmpeg (primeira execução):
   ```csharp
   await Xabe.FFmpeg.Downloader.GetLatestVersion(FFmpegVersion.Official);
   ```
3. Validar instalação executando comando teste:
   ```csharp
   var ffmpegPath = FFmpeg.ExecutablesPath;
   Console.WriteLine($"FFmpeg instalado em: {ffmpegPath}");
   ```
4. Documentar caminho onde FFmpeg será instalado (ex: `%USERPROFILE%\.ffmpeg`)

## Critérios de Aceite
- [ ] Xabe.FFmpeg (5.2.6) instalado em `src/VideoProcessor.Application`
- [ ] FFmpeg binário baixado e disponível
- [ ] Método helper `EnsureFFmpegInstalled()` criado
- [ ] Validação mostra caminho do FFmpeg no console
- [ ] Documentado em README onde FFmpeg é instalado

## Notas Técnicas
- Xabe.FFmpeg.Downloader faz download automático de FFmpeg
- Binários salvos em `%USERPROFILE%\.ffmpeg` por padrão
- Primeira execução pode levar ~30s para download (FFmpeg ~80MB)
- Alternativa: instalar FFmpeg manualmente e configurar PATH
