using fluXis.Modes.Gameplay.Lines;
using fluXis.Modes.Keys.Map.Objects.Drawable;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Modes.Keys.Gameplay.Objects;

public partial class KeysDrawableTimingLine : DrawableKeysHitObject<TimingLine>
{
    public override bool CanBeRemoved => Time.Current - Object.Time > 0;

    public KeysDrawableTimingLine([NotNull] TimingLine o)
        : base(o)
    {
        InternalChild = new Box { Height = 3 }.WithRelativeSize(Axes.X);
    }
}
