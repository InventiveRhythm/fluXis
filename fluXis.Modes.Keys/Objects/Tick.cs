using fluXis.Map.Structures;

namespace fluXis.Modes.Keys.Objects;

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
