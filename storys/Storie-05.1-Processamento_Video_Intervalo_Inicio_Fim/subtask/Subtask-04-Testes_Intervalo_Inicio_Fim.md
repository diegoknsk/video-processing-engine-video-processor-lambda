# Subtask 04: Testes unitários e validação do intervalo início/fim

## Descrição
Adicionar ou estender testes unitários do `VideoFrameExtractor` para cobrir os parâmetros opcionais de início e fim: comportamento com intervalo válido, backward compatibility (parâmetros omitidos), e validação de erros (start >= end, end > duração, start < 0).

## Passos de implementação
1. Revisar testes existentes em `VideoFrameExtractorTests.cs`; garantir que continuam passando quando a assinatura for estendida (ex.: passar null para start/end onde aplicável).
2. Adicionar testes para processamento com intervalo: mock ou vídeo de teste com duração conhecida; chamar com start e end válidos e verificar quantidade de frames e que os tempos estão dentro de [start, end].
3. Adicionar testes de validação: start >= end deve lançar; end maior que duração do vídeo deve lançar; start < 0 deve lançar (conforme implementação na Subtask 02).
4. Opcional: teste que sem start/end o resultado equivale ao comportamento anterior (mesma quantidade de frames para mesmo vídeo e intervalo).

## Formas de teste
- Executar `dotnet test` no projeto de testes; todos os testes devem passar.
- Cobertura: cenários de sucesso com intervalo, parâmetros omitidos e erros de validação devem estar cobertos.

## Critérios de aceite da subtask
- [ ] Testes unitários existentes continuam passando (backward compatibility).
- [ ] Há testes para extração com start/end válidos (quantidade de frames e intervalo correto).
- [ ] Há testes para argumentos inválidos: start >= end, end > duração, start < 0 (exceção esperada).
- [ ] `dotnet test` passa para o projeto VideoProcessor.Tests.Unit.
