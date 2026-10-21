using System;
using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Lists;

namespace fluXis.Modes.Gameplay.Objects;

#nullable enable

public abstract partial class DrawableHitObject : CompositeDrawable
{
    [Resolved]
    protected RulesetContainer Ruleset { get; private set; } = null!;

    [Resolved]
    protected ISkin Skin { get; private set; } = null!;

    public TrackedHitObject Tracked { get; }
    public HitObject Object { get; }
    public GameModeHitObjectManager Manager { get; internal set; } = null!;

    protected double TimeDelta => Object.Time - Time.Current;
    public abstract bool CanBeRemoved { get; }

    public bool Judged => Tracked.Judged;
    public HitWindows HitWindows => hitWindowLazy.Value;
    public Action<TrackedHitObject>? OnResult { get; set; }

    private readonly Lazy<HitWindows> hitWindowLazy;

    protected DrawableHitObject(HitObject o)
    {
        Object = o;
        Object.CreateNestedObjects();

        Tracked = new TrackedHitObject(o);
        hitWindowLazy = new Lazy<HitWindows>(() => Ruleset.PlayableMode.CreateHitWindowFor(Object));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Object.NestedObjects.ForEach(x =>
        {
            var d = CreateNested(x)
                    ?? throw new InvalidOperationException($"Can't create drawable representation of nested '{x}'.");

            d.Manager = Manager;
            d.OnResult += OnResult;
            Tracked.Nested.Add(d.Tracked);

            nestedObjects.Add(d);
            AddInternal(d);
        });
    }

    #region Nested

    public SlimReadOnlyListWrapper<DrawableHitObject> NestedObjects => nestedObjects.AsSlimReadOnly();
    private readonly List<DrawableHitObject> nestedObjects = [];

    protected virtual DrawableHitObject? CreateNested(HitObject obj) => null;

    #endregion

    public virtual void OnDestroy()
    {
        UpdateJudgement(false);
        NestedObjects.ForEach(x => x.OnDestroy());
        Object.CreateNestedObjects();
    }

    protected bool UpdateJudgement(bool byUser)
    {
        if (Judged)
            return false;

        CheckJudgement(byUser, TimeDelta);
        return Judged;
    }

    protected virtual void CheckJudgement(bool byUser, double offset) { }

    protected void ApplyResult(Judgement judgement, double? time = null)
    {
        if (Judged) throw new InvalidOperationException("Can not apply judgement to already judged HitObject.");

        Tracked.Judgement = judgement;
        Tracked.ClockTime = time ?? Time.Current;

        OnResult?.Invoke(Tracked);
    }
}

public abstract partial class DrawableHitObject<T> : DrawableHitObject
    where T : HitObject
{
    public new T Object => (T)base.Object;

    protected DrawableHitObject(T o)
        : base(o)
    {
    }
}
