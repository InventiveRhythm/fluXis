using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Modes.Objects;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes.Gameplay.Objects;

#nullable enable

public abstract partial class GameModeHitObjectManager : CompositeDrawable
{
    public Stack<HitObject> PastObjects { get; } = new();
    public List<DrawableHitObject> ActiveObjects { get; } = new();
    public List<HitObject> FutureObjects { get; } = new();

    protected GameModeHitObjectManager(IEnumerable<HitObject> objs)
    {
        FutureObjects.AddRange(objs.OrderBy(x => x.Time));
    }

    protected abstract DrawableHitObject? CreateDrawableFor(HitObject obj);
}
