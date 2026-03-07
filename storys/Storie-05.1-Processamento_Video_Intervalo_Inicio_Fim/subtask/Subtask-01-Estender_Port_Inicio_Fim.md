# Subtask 01: Estender port IVideoFrameExtractor com parâmetros opcionais de início e fim

## Descrição
Incluir no port `IVideoFrameExtractor` os parâmetros opcionais de tempo de início e tempo de fim do trecho a processar, de forma que a assinatura permita processar o vídeo inteiro (quando omitidos) ou apenas um intervalo em segundos.

## Passos de implementação
1. Abrir `IVideoFrameExtractor.cs` e definir a nova assinatura (ou overload): além de `videoPath`, `intervalSeconds` e `outputFolder`, aceitar `int? startTimeSeconds` e `int? endTimeSeconds` (ou um único método com parâmetros opcionais/default).
2. Documentar no XML: quando ambos são null/omitidos, processar o vídeo inteiro; quando informados, processar apenas de start até end (em segundos).
3. Garantir que o contrato não quebra chamadores existentes (ex.: parâmetros opcionais em C# ou overload que delega para o método completo com null).

## Formas de teste
- Compilar a solução e verificar que não há erros de referência nos projetos que já usam `IVideoFrameExtractor`.
- Executar testes existentes do `VideoFrameExtractor` (após implementação na Subtask 02) passando null para início/fim e validar mesmo resultado que hoje.
- Revisão de código: contrato claro e documentado.

## Critérios de aceite da subtask
- [ ] Interface `IVideoFrameExtractor` expõe parâmetros (ou overload) para tempo de início e fim opcionais em segundos.
- [ ] Documentação XML descreve o comportamento quando os valores são omitidos vs informados.
- [ ] Build da solução passa; chamadas existentes (CLI, testes) podem ser atualizadas sem quebrar o contrato (backward compatibility).
