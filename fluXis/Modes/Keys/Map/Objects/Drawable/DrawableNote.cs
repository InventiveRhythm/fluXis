using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Modes.Keys.Map.Objects.Drawable;

#nullable enable

public partial class DrawableNote : DrawableKeysHitObject<HitObject>
{
    public override bool CanBeRemoved => Judged || Time.Current - Object.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    public DrawableNote(HitObject o)
        : base(o)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetHitObject(PlayerLane, Manager.KeyCount).With(d => d.RelativeSizeAxes = Axes.X);
    }
}
