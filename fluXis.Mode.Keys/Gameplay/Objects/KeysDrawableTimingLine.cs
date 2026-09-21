using fluXis.Mode.Keys.Objects.Drawable;
using fluXis.Modes.Gameplay.Lines;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Mode.Keys.Gameplay.Objects;

public partial class KeysDrawableTimingLine : DrawableKeysHitObject<TimingLine>
{
    public override bool CanBeRemoved => Time.Current - Object.Time > 0;

    public KeysDrawableTimingLine([NotNull] TimingLine o)
        : base(o)
    {
        InternalChild = new Box { Height = 3 }.WithRelativeSize(Axes.X);
    }
}
