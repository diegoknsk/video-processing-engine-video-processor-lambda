Feature: Simulação de Erro por Duração de Vídeo
  Como desenvolvedor
  Quero que um vídeo com exatamente 1303 segundos dispare uma exceção de simulação
  Para demonstrar a propagação de erros por todas as camadas da aplicação

  Scenario: Vídeo com duração de 1303 segundos propaga exceção por todas as camadas
    Given um input válido de processamento de chunk
    When o extrator detecta que o vídeo tem 1303 segundos de duração
    Then o UseCase deve propagar a VideoDurationSimulationException sem capturá-la
    And a mensagem da exceção deve indicar que é uma simulação intencional

  Scenario: A constante de disparo da simulação é 1303 segundos
    Given a regra de negócio de simulação está configurada
    When consulto o valor de disparo da simulação
    Then o valor deve ser igual a 1303
