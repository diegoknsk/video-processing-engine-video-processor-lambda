# Subtask 04: Criar aplicação console CLI para teste local

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Criar novo projeto console (`VideoProcessor.CLI`) que permite testar a extração de frames localmente via linha de comando, recebendo parâmetros como caminho do vídeo, intervalo, e pasta de saída.

## Tarefas
1. Criar novo projeto console:
   ```bash
   dotnet new console -n VideoProcessor.CLI -o src/VideoProcessor.CLI
   ```
2. Adicionar referências:
   - `VideoProcessor.Application`
   - `VideoProcessor.Domain`
3. Implementar `Program.cs`:
   - Parser de argumentos: `--video`, `--interval`, `--output`
   - Validar argumentos obrigatórios
   - Instanciar `VideoFrameExtractor`
   - Chamar `ExtractFramesAsync()`
   - Exibir resultado formatado
4. Adicionar projeto à solution
5. Testar execução:
   ```bash
   dotnet run --project src/VideoProcessor.CLI -- --video sample.mp4 --interval 20 --output output/frames
   ```

## Critérios de Aceite
- [ ] Projeto `VideoProcessor.CLI` criado e adicionado à solution
- [ ] Argumentos de linha de comando funcionam: `--video`, `--interval`, `--output`
- [ ] Validação exibe erro amigável se argumentos obrigatórios faltam
- [ ] Execução processa vídeo e exibe resultado:
   ```
   Processando vídeo: sample.mp4
   Duração: 5m 30s
   Intervalo: 20s
   Frames esperados: 17
   ---
   Extraindo frames... ████████████████████ 100%
   ---
   ✓ Processamento concluído!
   Total de frames: 17
   Pasta de saída: output/frames/
   Tempo de processamento: 8.5s
   ```
- [ ] Frames salvos na pasta especificada

## Notas Técnicas
- Usar `args.Contains("--video")` para parsing simples
- Alternativa: usar biblioteca System.CommandLine para parsing robusto
- Validar que arquivo de vídeo existe antes de processar
- Usar `Console.Write("\r...")` para progresso na mesma linha
