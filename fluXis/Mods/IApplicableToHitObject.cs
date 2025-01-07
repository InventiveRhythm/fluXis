using fluXis.Map.Structures;

namespace fluXis.Mods;

public interface IApplicableToHitObject
{
    void Apply(HitObject hit);
}
