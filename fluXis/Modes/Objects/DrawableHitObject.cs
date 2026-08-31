using fluXis.Map.Structures;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes.Objects;

public partial class DrawableHitObject : CompositeDrawable
{
}

public partial class DrawableHitObject<T> : DrawableHitObject
    where T : HitObject
{
}
