using System;
using fluXis.Map.Structures;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes.Gameplay.Objects;

#nullable enable

public abstract partial class DrawableHitObject : CompositeDrawable
{
    [Resolved]
    protected RulesetContainer Ruleset { get; private set; } = null!;

    [Resolved]
    protected ISkin Skin { get; private set; } = null!;

    public HitObject Object { get; }
    public GameModeHitObjectManager Manager { get; internal set; } = null!;

    protected double TimeDelta => Object.Time - Time.Current;
    public abstract bool CanBeRemoved { get; }

    public bool Judged { get; protected set; }
    public HitWindows HitWindows => hitWindowLazy.Value;
    public Action<DrawableHitObject, double>? OnHit { get; set; }

    private readonly Lazy<HitWindows> hitWindowLazy;

    protected DrawableHitObject(HitObject o)
    {
        Object = o;
        hitWindowLazy = new Lazy<HitWindows>(() => Ruleset.PlayableMode.CreateHitWindowFor(Object));
    }

    public virtual void OnDestroy() => UpdateJudgement(false);

    protected bool UpdateJudgement(bool byUser)
    {
        if (Judged)
            return false;

        CheckJudgement(byUser, TimeDelta);
        return Judged;
    }

    protected virtual void CheckJudgement(bool byUser, double offset) { }

    protected void ApplyResult(double diff)
    {
        if (Judged) throw new InvalidOperationException("Can not apply judgement to already judged HitObject.");

        Judged = true;
        OnHit?.Invoke(this, diff);
    }
}

public abstract partial class DrawableHitObject<T> : DrawableHitObject
    where T : HitObject
{
    public new HitObject Object => (T)base.Object;

    protected DrawableHitObject(T o)
        : base(o)
    {
    }
}
