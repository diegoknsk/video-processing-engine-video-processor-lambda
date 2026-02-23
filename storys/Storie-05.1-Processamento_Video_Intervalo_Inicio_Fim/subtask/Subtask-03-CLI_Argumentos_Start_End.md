# Subtask 03: Atualizar CLI com argumentos --start e --end opcionais

## Descrição
Incluir na aplicação console (VideoProcessor.CLI) os argumentos opcionais `--start` e `--end` (em segundos) e repassá-los ao `ExtractFramesAsync`, permitindo processar um trecho do vídeo a partir da linha de comando.

## Passos de implementação
1. No parsing de argumentos da CLI (ex.: `System.CommandLine` ou manual), adicionar opções `--start` e `--end` opcionais, aceitando inteiros (segundos).
2. Se `--start` ou `--end` forem omitidos, passar null (ou equivalente) para o extrator, mantendo comportamento de processar o vídeo inteiro.
3. Chamar `ExtractFramesAsync` com os novos parâmetros quando informados.
4. Documentar no README ou help da CLI o uso: exemplo para processar do 0s ao 59s e outro do 60s ao 119s (um processo por minuto em vídeo de 10 min).

## Formas de teste
- Executar `dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output out/` (sem start/end): deve processar o vídeo inteiro como hoje.
- Executar com `--start 0 --end 59`: deve gerar frames apenas no primeiro minuto.
- Executar com `--start 60 --end 119`: deve gerar frames apenas no segundo minuto; verificar contagem e nomes dos arquivos.

## Critérios de aceite da subtask
- [ ] CLI aceita `--start N` e `--end N` (opcionais); valores em segundos.
- [ ] Sem `--start`/`--end`, o comportamento é igual ao atual (vídeo inteiro).
- [ ] Com `--start` e `--end`, apenas o trecho indicado é processado e os frames aparecem na pasta de saída.
- [ ] Documentação (README ou --help) inclui exemplo de uso com intervalo (ex.: 0–59 e 60–119).
