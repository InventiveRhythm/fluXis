using fluXis.Map.Structures;
using fluXis.Modes.Gameplay.Objects;
using fluXis.Modes.Keys.Gameplay;
using osu.Framework.Graphics;

namespace fluXis.Modes.Keys.Map.Objects.Drawable;

#nullable enable

public abstract partial class DrawableKeysHitObject<T> : DrawableHitObject<T>
    where T : HitObject
{
    protected new KeysHitObjectManager Manager => (KeysHitObjectManager)base.Manager;

    protected int PlayerLane
    {
        get
        {
            var lane = Object.Lane;

            while (lane > Manager.KeyCount)
                lane -= Manager.KeyCount;

            return lane;
        }
    }

    protected DrawableKeysHitObject(T o)
        : base(o)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;
        Masking = true;
    }

    protected override void Update()
    {
        base.Update();

        Y = Manager.PositionAtTime(Object.Time);
    }
}
