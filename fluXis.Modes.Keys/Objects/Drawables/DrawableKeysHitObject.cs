using System;
using System.Linq;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Modes.Gameplay.Objects;
using fluXis.Modes.Keys.Gameplay;
using fluXis.Modes.Keys.Gameplay.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Modes.Keys.Objects.Drawables;

public abstract partial class DrawableKeysHitObject<T> : DrawableHitObject<T>
    where T : HitObject
{
    protected new KeysHitObjectManager Manager => (KeysHitObjectManager)base.Manager;

    [Resolved]
    protected KeysKeybindContainer Keybinds { get; private set; } = null!;

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

    public FluXisGameplayKeybind Action { get; set; }

    public double ScrollVelocityTime { get; private set; }
    public Easing ScrollEasing { get; private set; } = Easing.None;

    protected DrawableKeysHitObject(T o)
        : base(o)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;
        Masking = true;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Action = Keybinds.Keys[Math.Clamp(Object.Lane - 1, 0, Keybinds.Keys.Length - 1)];

        var group = Object.ScrollGroup ?? Manager.DefaultScrollGroup;
        ScrollVelocityTime = group.PositionFromTime(Object.Time);
        ScrollEasing = Ruleset.Map.ObjectsOfType<HitObjectEaseEvent>().LastOrDefault(x => x.Time <= Object.Time)?.Easing ?? Easing.None;
    }

    protected override void Update()
    {
        base.Update();
        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        Y = Manager.PositionAtTime(ScrollVelocityTime, Object.ScrollGroup, ScrollEasing);
    }
}
