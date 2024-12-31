using fluXis.Map;

namespace fluXis.Mods;

public interface IApplicableToEvents
{
    void Apply(MapEvents events);
}
