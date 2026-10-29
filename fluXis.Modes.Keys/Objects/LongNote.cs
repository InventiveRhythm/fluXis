using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;

namespace fluXis.Modes.Keys.Objects;

public class LongNote : HitObject, IHasDuration
{
    public override int ComboContribution => 2;

    public double Duration { get; set; }

    protected override void CreateNested()
    {
        AddNested(new LongNoteTail { Time = this.GetEndTime(), Lane = Lane, Group = Group });
    }
}
