# Subtask 04: Integrar use case no handler Lambda e processar vídeo em /tmp

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Integrar `ProcessVideoUseCase` no handler Lambda, receber input com vídeo (base64 ou mock), salvar em /tmp, processar, e retornar resultado estruturado.

## Tarefas
1. Atualizar `src/VideoProcessor.Lambda/Function.cs`:
   - Definir input: `{ "videoBase64": "...", "intervalSeconds": 20 }` (ou simular arquivo)
   - Salvar vídeo em `/tmp/input.mp4`
   - Chamar `ProcessVideoUseCase.Execute(videoPath, interval, "/tmp/frames")`
   - Retornar output: `{ "status": "SUCCEEDED", "framesCount": X, "processingDuration": "Y", "message": "..." }`
2. Para teste inicial: usar vídeo mock pequeno embutido ou simular frames
3. Configurar DI para injetar `ProcessVideoUseCase` e `VideoFrameExtractor`
4. Aumentar configuração Lambda:
   - Timeout: 300s
   - Memória: 2048MB
   - Ephemeral storage (/tmp): 1024MB

## Critérios de Aceite
- [ ] Handler recebe input estruturado com videoBase64 e intervalSeconds
- [ ] Vídeo salvo em `/tmp/input.mp4`
- [ ] Use case chamado e executado
- [ ] Frames gerados em `/tmp/frames/`
- [ ] Output retorna: status, framesCount, processingDuration, message
- [ ] Log CloudWatch mostra: "Processando vídeo... Frames gerados: X"
- [ ] Lambda configurado com timeout 300s, memória 2048MB, /tmp 1024MB
- [ ] Execução testável via Console AWS (payload manual)

## Notas Técnicas
- Para teste inicial: pode simular processamento sem vídeo real (criar frames vazios)
- Base64 para vídeo: `Convert.FromBase64String(input.videoBase64)`
- Alternativa: receber S3 key (implementar na Storie-07)
- DI: usar construtor primário no Function.cs
