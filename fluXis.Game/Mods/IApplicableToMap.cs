using fluXis.Game.Map;

namespace fluXis.Game.Mods;

public interface IApplicableToMap
{
    void Apply(MapInfo map);
}
