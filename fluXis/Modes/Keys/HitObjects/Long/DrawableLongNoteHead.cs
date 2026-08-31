using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Modes.Keys.HitObjects.Long;

public partial class DrawableLongNoteHead : DrawableLongNotePart
{
    public bool Hittable => Time.Current - Data.Time > -HitWindows.TimingFor(HitWindows.Lowest);

    public DrawableLongNoteHead(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetLongNoteStart(VisualLane, ObjectManager.KeyCount).With(d => d.RelativeSizeAxes = Axes.X);
    }
}
