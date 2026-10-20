using fluXis.Map.Structures;
using rhym;

namespace fluXis.Mode.Keys.Objects;

[ResourceLocation("flux:keys/mine")]
public class Landmine : HitObject
{
    public override float DensityContribution => 0;

    public bool Hidden { get; set; }

    public override string? GetSample(bool hit) => null;

    public override bool OnEditorMiddleClick()
    {
        Hidden = !Hidden;
        return true;
    }
}
