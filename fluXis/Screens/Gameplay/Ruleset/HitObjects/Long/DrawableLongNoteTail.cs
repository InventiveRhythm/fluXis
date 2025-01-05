using fluXis.Map.Structures;
using fluXis.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects.Long;

public partial class DrawableLongNoteTail : DrawableLongNotePart
{
    protected override double HitTime => Data.EndTime;
    protected override HitWindows HitWindows => Ruleset.ReleaseWindows;

    public DrawableLongNoteTail(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = SkinManager.GetLongNoteEnd(Data.Lane, ObjectManager.KeyCount).With(d =>
        {
            d.Anchor = Anchor.BottomCentre;
            d.Origin = Anchor.BottomCentre;
        });
    }
}
