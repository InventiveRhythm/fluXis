using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Lists;

namespace fluXis.Modes.Gameplay.Objects;

#nullable enable

public abstract partial class GameModeHitObjectManager : CompositeDrawable
{
    protected virtual int MinimumLoadedHitObject => 3;
    public virtual bool Finished => NestedManagers.All(x => x.Finished) && ActiveObjects.Count == 0 && FutureObjects.Count == 0;

    [Resolved]
    protected GameModePlayer Player { get; private set; } = null!;

    [Resolved]
    protected Playfield Playfield { get; private set; } = null!;

    protected RulesetContainer Ruleset { get; }
    protected PlayableMap Map { get; }

    [UsedImplicitly]
    public double VisualTimeOffset { get; set; }

    protected GameModeHitObjectManager(RulesetContainer ruleset, PlayableMap map)
    {
        Ruleset = ruleset;
        Map = map;

        AlwaysPresent = true;
        ActiveObjects = new Container<DrawableHitObject>().WithRelativeSize(Axes.Both);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        HitObject? last = null;

        foreach (var hit in FutureObjects)
        {
            last?.NextObject = hit;
            last = hit;

            if (!string.IsNullOrWhiteSpace(hit.Group) && Ruleset.ScrollGroups.TryGetValue(hit.Group, out var gr))
                hit.ScrollGroup = gr;
        }

        AddInternal(ActiveObjects);
    }

    protected override void Update()
    {
        base.Update();

        while (FutureObjects is { Count: > 0 } && (ShouldBeRendered(FutureObjects[0]) || ActiveObjects.Count < MinimumLoadedHitObject))
        {
            var hit = FutureObjects[0];
            var draw = createObject(hit);

            FutureObjects.RemoveAt(0);

            if (draw is null) // this will break stuff 100%, but it won't freeze the game
                PastObjects.Push(new TrackedHitObject(hit));
            else
                ActiveObjects.Add(draw);
        }

        while (ActiveObjects.Count > 0 && !ShouldBeRendered(ActiveObjects[0].Object) && ActiveObjects.Count > MinimumLoadedHitObject)
        {
            var hit = ActiveObjects[0];
            removeObject(hit, true);
        }

        foreach (var hitObject in ActiveObjects.Where(h => h.CanBeRemoved).ToList())
            removeObject(hitObject);

        while (Ruleset.AllowReverting && PastObjects.Count > 0)
        {
            var result = PastObjects.Peek();

            if (Clock.CurrentTime >= result.ClockTime)
                break;

            revertObject(PastObjects.Pop());
        }
    }

    #region Objects

    protected Stack<TrackedHitObject> PastObjects { get; } = [];
    protected Container<DrawableHitObject> ActiveObjects { get; }
    protected List<HitObject> FutureObjects { get; } = [];

    protected abstract bool ShouldBeRendered(HitObject obj);
    protected abstract DrawableHitObject? CreateDrawableFor(HitObject obj);

    private DrawableHitObject? createObject(HitObject obj)
    {
        var draw = CreateDrawableFor(obj);
        if (draw is null) return null;

        draw.Depth = (float)obj.Time;
        draw.Manager = this;
        draw.OnResult += onResult;
        return draw;
    }

    private void onResult(TrackedHitObject tracked)
    {
        if (Playfield.IsSubPlayfield)
            return;

        if (Player.HealthProcessor.Failed)
            return;

        Player.JudgementProcessor.AddResult(tracked);
    }

    private void removeObject(DrawableHitObject draw, bool addToFuture = false)
    {
        if (!addToFuture)
            draw.OnDestroy();

        draw.OnResult -= onResult;

        if (addToFuture) FutureObjects.Insert(0, draw.Object);
        else PastObjects.Push(draw.Tracked);

        ActiveObjects.Remove(draw, true);
    }

    private void revertObject(TrackedHitObject track)
    {
        if (!Playfield.IsSubPlayfield)
        {
            Player.JudgementProcessor.RevertResult(track);

            /*if (obj.HoldEndResult is not null)
                Player.JudgementProcessor.RevertResult(obj.HoldEndResult.Value);

            if (obj.Result is not null)
                Player.JudgementProcessor.RevertResult(obj.Result.Value);*/
        }

        var draw = createObject(track.Object);
        if (draw is null) return;

        ActiveObjects.Add(draw);
    }

    #endregion

    #region Nesting

    public bool IsNested { get; private set; }
    public SlimReadOnlyListWrapper<GameModeHitObjectManager> NestedManagers => nestedManagers.AsSlimReadOnly();
    private readonly List<GameModeHitObjectManager> nestedManagers = [];

    protected void AddNestedManager(GameModeHitObjectManager manager)
    {
        manager.IsNested = true;
        nestedManagers.Add(manager);
    }

    #endregion
}
