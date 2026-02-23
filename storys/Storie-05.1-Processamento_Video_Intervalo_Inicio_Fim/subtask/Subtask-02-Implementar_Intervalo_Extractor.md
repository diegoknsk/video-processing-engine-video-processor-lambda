# Subtask 02: Implementar processamento por intervalo no VideoFrameExtractor

## Descrição
Implementar em `VideoFrameExtractor` a lógica que, quando início e/ou fim forem informados, limita a extração ao trecho [start, end] do vídeo (em segundos), calculando os instantes de captura apenas nesse intervalo e validando os parâmetros.

## Passos de implementação
1. Atualizar o método `ExtractFramesAsync` para aceitar os parâmetros opcionais de início e fim (conforme port da Subtask 01).
2. Se início/fim forem informados: validar start >= 0, start < end, e end <= duração do vídeo (ou end não exceder duração); lançar `ArgumentOutOfRangeException` ou similar com mensagem clara quando inválido.
3. Calcular a lista de instantes de captura apenas dentro de [start, end]: primeiro frame em start (ou no primeiro múltiplo do intervalo >= start), depois a cada intervalSeconds até <= end.
4. Manter nomenclatura dos arquivos consistente (ex.: frame_0001_Xs.jpg onde X é o tempo em segundos no vídeo).
5. Quando início e fim forem omitidos, manter exatamente o comportamento atual (vídeo inteiro).

## Formas de teste
- Teste manual: vídeo de 120s, interval 20, start=0, end=59 → esperado frames em 0s e 20s e 40s (não 60s).
- Teste manual: mesmo vídeo sem start/end → mesmo número de frames que antes (0, 20, 40, 60, 80, 100).
- Testes unitários: cenários de validação (start >= end, end > duração, start < 0) e cenário de sucesso com intervalo.

## Critérios de aceite da subtask
- [ ] Com start e end informados, apenas o trecho [start, end] é considerado para extração; quantidade de frames e instantes corretos.
- [ ] Validação rejeita start >= end, end > duração do vídeo e start < 0 com exceção e mensagem adequada.
- [ ] Com start e end omitidos (null), resultado idêntico ao comportamento atual da Storie-05.
- [ ] Nomes dos arquivos de frame permanecem no padrão existente (ex.: frame_0001_0s.jpg).
