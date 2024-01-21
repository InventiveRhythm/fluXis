using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects.Long;

public partial class DrawableLongNoteHead : DrawableLongNotePart
{
    public DrawableLongNoteHead(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = SkinManager.GetHitObject(Data.Lane, ObjectManager.KeyCount).With(d =>
        {
            d.Anchor = Anchor.BottomCentre;
            d.Origin = Anchor.BottomCentre;
        });
    }
}
