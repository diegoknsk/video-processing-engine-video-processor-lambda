namespace VideoProcessor.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando a duração total do vídeo é exatamente 1303 segundos.
/// Regra de simulação intencional para demonstrar a propagação de exceções
/// por todas as camadas da aplicação (Domain → Application → Lambda).
/// </summary>
public class VideoDurationSimulationException(double durationSeconds)
    : Exception($"[SIMULAÇÃO] A duração do vídeo ({durationSeconds:F0}s) ativou a regra de simulação de erro. " +
                "Esta exceção é intencional para fins de demonstração.")
{
    public double DurationSeconds { get; } = durationSeconds;

    /// <summary>Duração mágica que dispara a simulação (em segundos).</summary>
    public const int TriggerDurationSeconds = 1303;
}
