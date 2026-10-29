using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using rhym;

namespace fluXis.Mode.Keys.Objects;

[ResourceLocation("flux:keys/long")]
public class LongNote : HitObject, IHasDuration
{
    public override int ComboContribution => 2;

    public double Duration { get; set; }

    protected override void CreateNested()
    {
        AddNested(new LongNoteTail { Time = this.GetEndTime(), Lane = Lane, Group = Group });
    }
}
