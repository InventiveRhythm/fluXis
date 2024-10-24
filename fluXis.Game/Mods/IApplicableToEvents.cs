using fluXis.Game.Map;

namespace fluXis.Game.Mods;

public interface IApplicableToEvents
{
    void Apply(MapEvents events);
}
