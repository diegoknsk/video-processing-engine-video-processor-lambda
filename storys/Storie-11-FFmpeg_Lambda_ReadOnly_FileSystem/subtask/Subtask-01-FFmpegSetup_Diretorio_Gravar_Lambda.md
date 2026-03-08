# Subtask 01: Usar diretório gravável em Lambda no FFmpegSetup

## Descrição
Alterar `FFmpegSetup.EnsureFFmpegInstalledAsync()` para que, quando executado em AWS Lambda, use `/tmp` como base do diretório `.ffmpeg` em vez de `Environment.SpecialFolder.UserProfile` (que no Lambda resolve para `/var/task`, read-only).

## Passos de implementação
1. Em `FFmpegSetup.cs`, definir a base do diretório FFmpeg de forma condicional: se a variável de ambiente `AWS_LAMBDA_FUNCTION_NAME` (ou `LAMBDA_TASK_ROOT`) estiver definida, usar `/tmp`; caso contrário, usar `Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)`.
2. Construir o caminho do diretório FFmpeg com `Path.Combine(basePath, ".ffmpeg")` e usar esse valor em `Directory.CreateDirectory`, `FFmpegDownloader.GetLatestVersion` e `FFmpeg.SetExecutablesPath`.
3. Manter o restante da lógica (verificação de path já configurado, download e configuração) inalterado.
4. Rodar testes existentes para garantir que não há regressão em ambiente local.

## Formas de teste
- Executar a CLI ou testes locais que dependem de FFmpeg e confirmar que o comportamento em Windows continua o mesmo (UserProfile\.ffmpeg).
- Publicar o Lambda e invocar na AWS; verificar que não ocorre "Read-only file system" (com ou sem Layer).

## Critérios de aceite da subtask
- [ ] Em Lambda, nenhuma escrita é tentada em `/var/task`; o diretório usado para .ffmpeg em Lambda é `/tmp/.ffmpeg` (ou equivalente).
- [ ] Em ambiente não-Lambda (local), o diretório continua sendo UserProfile\.ffmpeg.
- [ ] Testes unitários/existentes do projeto continuam passando.
