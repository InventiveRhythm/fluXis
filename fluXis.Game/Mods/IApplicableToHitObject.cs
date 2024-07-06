using fluXis.Game.Map.Structures;

namespace fluXis.Game.Mods;

public interface IApplicableToHitObject
{
    void Apply(HitObject hit);
}
