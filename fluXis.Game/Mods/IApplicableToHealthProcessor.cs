using fluXis.Game.Scoring.Processing.Health;

namespace fluXis.Game.Mods;

public interface IApplicableToHealthProcessor
{
    void Apply(HealthProcessor processor);
}
