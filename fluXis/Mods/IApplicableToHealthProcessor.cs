using fluXis.Scoring.Processing.Health;

namespace fluXis.Mods;

public interface IApplicableToHealthProcessor
{
    void Apply(HealthProcessor processor);
}
