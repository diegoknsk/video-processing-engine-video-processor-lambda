# Subtask 03: Validar local e na AWS e atualizar documentação

## Descrição
Validar que a correção funciona localmente e na AWS (com Layer), e atualizar a documentação para deixar explícito que em Lambda o único diretório gravável é `/tmp` e que o uso de Lambda Layer com FFmpeg é a abordagem recomendada.

## Passos de implementação
1. Executar testes automatizados do projeto (unit e, se houver, integração) e garantir que todos passam.
2. Testar localmente a CLI ou o Lambda Test Tool com um vídeo de exemplo; confirmar que o FFmpeg continua sendo encontrado/baixado em UserProfile\.ffmpeg.
3. Fazer deploy do Lambda (com Layer FFmpeg anexado à função) e invocar com payload válido; confirmar que o processamento retorna sucesso sem "Read-only file system".
4. Em `docs/deploy-lambda-video-processor.md` ou README (ou equivalente), adicionar ou reforçar: (a) em Lambda, apenas `/tmp` é gravável; (b) a aplicação usa `/tmp` para fallback de FFmpeg quando não há Layer; (c) recomenda-se configurar um Lambda Layer com FFmpeg em `/opt/ffmpeg` ou `/opt/bin`.

## Formas de teste
- `dotnet test` em todo o repositório.
- Invocação manual do Lambda na AWS após deploy (com Layer) e verificação da resposta e logs no CloudWatch.

## Critérios de aceite da subtask
- [ ] Testes do projeto passam.
- [ ] Lambda na AWS (com Layer) processa um chunk com sucesso e retorna resposta coerente.
- [ ] Documentação atualizada menciona `/tmp` como diretório gravável no Lambda e Layer como recomendação para FFmpeg.
