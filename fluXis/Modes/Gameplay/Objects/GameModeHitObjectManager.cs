using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes.Gameplay.Objects;

#nullable enable

public abstract partial class GameModeHitObjectManager : CompositeDrawable
{
    protected virtual int MinimumLoadedHitObject => 3;

    [Resolved]
    protected GameModePlayer Player { get; private set; } = null!;

    [Resolved]
    protected Playfield Playfield { get; private set; } = null!;

    protected RulesetContainer Ruleset { get; }
    protected MapInfo Map { get; }
    protected MapEvents Events { get; }

    protected GameModeHitObjectManager(RulesetContainer ruleset, MapInfo map, MapEvents events)
    {
        Ruleset = ruleset;
        Map = map;
        Events = events;

        AlwaysPresent = true;
        ActiveObjects = new Container<DrawableHitObject>().WíthRelativeSize(Axes.Both);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(ActiveObjects);
    }

    protected override void Update()
    {
        base.Update();

        while (FutureObjects is { Count: > 0 } && (ShouldBeRendered(FutureObjects[0]) || ActiveObjects.Count < MinimumLoadedHitObject))
        {
            var hit = createObject(FutureObjects[0]);

            FutureObjects.RemoveAt(0);

            if (hit is null) // this will break stuff 100%, but it won't freeze the game
                PastObjects.Push(FutureObjects[0]);
            else
                ActiveObjects.Add(hit);
        }

        while (ActiveObjects.Count > 0 && !ShouldBeRendered(ActiveObjects[^1].Object) && ActiveObjects.Count > MinimumLoadedHitObject)
        {
            var hit = ActiveObjects[^1];
            removeObject(hit, true);
        }

        foreach (var hitObject in ActiveObjects.Where(h => h.CanBeRemoved).ToList())
            removeObject(hitObject);

        while (Ruleset.AllowReverting && PastObjects.Count > 0)
        {
            var result = PastObjects.Peek().Result;

            if (result is null || Clock.CurrentTime >= result.Value.Time)
                break;

            revertObject(PastObjects.Pop());
        }
    }

    #region Objects

    protected Stack<HitObject> PastObjects { get; } = [];
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
        // draw.OnHit += hit;
        return draw;
    }

    private void removeObject(DrawableHitObject draw, bool addToFuture = false)
    {
        /*if (!addToFuture)
            obj.OnKill();*/

        // obj.OnHit -= hit;

        if (addToFuture) FutureObjects.Insert(0, draw.Object);
        else PastObjects.Push(draw.Object);

        ActiveObjects.Remove(draw, true);
    }

    private void revertObject(HitObject obj)
    {
        if (!Playfield.IsSubPlayfield)
        {
            if (obj.HoldEndResult is not null)
                Player.JudgementProcessor.RevertResult(obj.HoldEndResult.Value);

            if (obj.Result is not null)
                Player.JudgementProcessor.RevertResult(obj.Result.Value);
        }

        var draw = createObject(obj);
        if (draw is null) return;

        ActiveObjects.Add(draw);
    }

    #endregion

    #region Nesting

    public bool IsNested { get; private set; }
    private readonly List<GameModeHitObjectManager> nestedManagers = [];

    protected void AddNestedManager(GameModeHitObjectManager manager)
    {
        manager.IsNested = true;
        nestedManagers.Add(manager);
    }

    #endregion
}
