using fluXis.Map.Structures;

namespace fluXis.Mode.Keys.Objects;

public class Tick : HitObject
{
    public override float DensityContribution => 0.1f;

    public bool Small { get; set; }

    public override bool OnEditorMiddleClick()
    {
        Small = !Small;
        return true;
    }
}
