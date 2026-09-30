using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;

namespace fluXis.Mode.Keys.Objects;

public class LongNote : HitObject, IHasDuration
{
    public double Duration { get; set; }

    public override void CreateNested()
    {
        base.CreateNested();
        AddNested(new LongNoteTail { Time = this.GetEndTime(), Lane = Lane, Group = Group });
    }
}
