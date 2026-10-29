using fluXis.Map.Structures;
using rhym;

namespace fluXis.Mode.Keys.Objects;

[ResourceLocation("flux:keys/tick")]
public class Tick : HitObject
{
    public override float DensityContribution => 0.1f;

    public bool Small { get; set; }

    public override string? GetSample(bool hit)
    {
        if (!hit) return null;

        if (string.IsNullOrWhiteSpace(Sample) || Sample == ":normal")
            return Small ? ":tick-small" : ":tick-big";

        return Sample;
    }

    public override bool OnEditorMiddleClick()
    {
        Small = !Small;
        return true;
    }
}
