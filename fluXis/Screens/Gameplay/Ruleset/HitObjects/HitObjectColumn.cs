using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Structs;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class HitObjectColumn : Container<DrawableHitObject>
{
    private const float minimum_loaded_hit_objects = 3;

    [Resolved]
    public Playfield Playfield { get; private set; }

    [Resolved]
    private PlayfieldPlayer player { get; set; }

    [Resolved]
    private RulesetContainer ruleset { get; set; }

    public Stack<HitObject> PastHitObjects { get; } = new();
    public List<HitObject> FutureHitObjects { get; } = new();
    public List<DrawableHitObject> HitObjects { get; } = new();

    public bool Finished => HitObjects.Count == 0 && FutureHitObjects.Count == 0;

    [CanBeNull]
    public HitObject NextUp
    {
        get
        {
            if (HitObjects.Count > 0)
                return HitObjects[0].Data;

            return FutureHitObjects.Count > 0 ? FutureHitObjects[0] : null;
        }
    }

    public MapInfo Map { get; }
    public HitObjectManager HitManager { get; }
    public int Lane { get; }

    private static int[] snaps { get; } = { 48, 24, 16, 12, 8, 6, 4, 3 };
    private Dictionary<int, int> snapIndices { get; } = new();

    public ScrollGroup DefaultScrollGroup { get; }

    private JudgementProcessor judgementProcessor => player.JudgementProcessor;
    private DependencyContainer dependencies;

    public HitObjectColumn(MapInfo map, RulesetContainer ruleset, HitObjectManager hitManager, int lane)
    {
        Map = map;
        Lane = lane;
        HitManager = hitManager;

        var idx = Lane;

        if (map.IsSplit && idx > map.RealmEntry!.KeyCount)
            idx -= map.RealmEntry!.KeyCount;

        DefaultScrollGroup = ruleset.ScrollGroups[$"${idx}"];

        var objects = Map.HitObjects.Where(h => h.Lane == Lane).ToList();
        objects.Sort((a, b) => a.Time.CompareTo(b.Time));
        objects.ForEach(FutureHitObjects.Add);

        HitObject last = null;

        foreach (var hit in FutureHitObjects)
        {
            if (last != null)
                last.NextObject = hit;

            hit.StartEasing = HitManager.EasingAtTime(hit.Time);
            hit.EndEasing = HitManager.EasingAtTime(hit.EndTime);
            last = hit;

            if (!string.IsNullOrWhiteSpace(hit.Group) && ruleset.ScrollGroups.TryGetValue(hit.Group, out var gr))
                hit.ScrollGroup = gr;
        }

        initSnapIndices();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        dependencies.CacheAs(this);
    }

    protected override void Update()
    {
        base.Update();

        while (FutureHitObjects is { Count: > 0 } && (ShouldDisplay(FutureHitObjects[0].Time, FutureHitObjects[0].ScrollGroup) || HitObjects.Count < minimum_loaded_hit_objects))
        {
            var hit = createHitObject(FutureHitObjects[0]);
            HitObjects.Add(hit);
            AddInternal(hit);

            FutureHitObjects.RemoveAt(0);
        }

        while (HitObjects.Count > 0 && !ShouldDisplay(HitObjects.Last().Data.Time, HitObjects.Last().Data.ScrollGroup) && HitObjects.Count > minimum_loaded_hit_objects)
        {
            var hit = HitObjects.Last();
            removeHitObject(hit, true);
        }

        foreach (var hitObject in HitObjects.Where(h => h.CanBeRemoved).ToList())
            removeHitObject(hitObject);

        while (ruleset.AllowReverting && PastHitObjects.Count > 0)
        {
            var result = PastHitObjects.Peek().Result;

            if (result is null || Clock.CurrentTime >= result.Value.Time)
                break;

            revertHitObject(PastHitObjects.Pop());
        }
    }

    public bool ShouldDisplay(double time, [CanBeNull] ScrollGroup group = null)
    {
        group ??= DefaultScrollGroup;

        var svTime = group.PositionFromTime(time);
        var y = PositionAtTime(svTime, group);
        return y >= 0;
    }

    public Vector2 FullPositionAt(double time, float lane, [CanBeNull] ScrollGroup group = null, Easing ease = Easing.None)
        => new(HitManager.PositionAtLane(lane), PositionAtTime(time, group, ease));

    public float PositionAtTime(double time, [CanBeNull] ScrollGroup group = null, Easing ease = Easing.None)
    {
        group ??= DefaultScrollGroup;

        var pos = HitManager.HitPosition;
        var current = group.CurrentTime + HitManager.VisualTimeOffset;
        var y = (float)(pos - .5f * ((time - (float)current) * (HitManager.ScrollSpeed * group.ScrollMultiplier)));

        if (ease <= Easing.None || y < 0 || y > pos)
            return y;

        var progress = y / pos;
        y = Interpolation.ValueAt(progress, 0, pos, 0, 1, ease);
        return float.IsFinite(y) ? y : 0;
    }

    public bool IsFirst(DrawableHitObject hitObject) => HitObjects.FirstOrDefault(h => h.Data.Lane == hitObject.Data.Lane && h.Data.Time < hitObject.Data.Time) == null;

    public int GetSnapIndex(double time)
    {
        if (snapIndices.TryGetValue((int)time, out int i))
            return i;

        var closest = snapIndices.Keys.MinBy(k => Math.Abs(k - time));

        // allow a 10ms margin of error for snapping
        if (Math.Abs(closest - time) <= 10 && snapIndices.TryGetValue(closest, out i))
            return i;

        // still nothing...
        return -1;
    }

    protected override int Compare(Drawable x, Drawable y)
    {
        var a = (DrawableHitObject)x;
        var b = (DrawableHitObject)y;

        var result = a.Data.Time.CompareTo(b.Data.Time);

        if (result != 0)
            return result;

        result = a.Data.Lane.CompareTo(b.Data.Lane);

        if (result != 0)
            return result;

        return a.Data.GetHashCode().CompareTo(b.Data.GetHashCode());
    }

    private void revertHitObject(HitObject hit)
    {
        if (!Playfield.IsSubPlayfield)
        {
            if (hit.HoldEndResult is not null)
                judgementProcessor.RevertResult(hit.HoldEndResult.Value);

            if (hit.Result is not null)
                judgementProcessor.RevertResult(hit.Result.Value);
        }

        var draw = createHitObject(hit);
        HitObjects.Insert(0, draw);
        AddInternal(draw);
    }

    private DrawableHitObject createHitObject(HitObject data)
    {
        var draw = HitManager.CreateHitObject(data);
        draw.OnHit += hit;
        return draw;
    }

    private void removeHitObject(DrawableHitObject hitObject, bool addToFuture = false)
    {
        if (!addToFuture)
        {
            hitObject.OnKill();
        }

        hitObject.OnHit -= hit;

        HitObjects.Remove(hitObject);

        if (addToFuture)
            FutureHitObjects.Insert(0, hitObject.Data);
        else
            PastHitObjects.Push(hitObject.Data);

        RemoveInternal(hitObject, true);
    }

    private void hit(DrawableHitObject hitObject, double difference)
    {
        if (Playfield.IsSubPlayfield)
            return;

        // since judged is only set after hitting the tail this works
        var isHoldEnd = hitObject is DrawableLongNote { Judged: true };

        var hitWindows = isHoldEnd ? ruleset.ReleaseWindows : ruleset.HitWindows;
        var judgement = hitWindows.JudgementFor(difference);

        if (player.HealthProcessor.Failed)
            return;

        var result = new HitResult(Time.Current, difference, judgement, isHoldEnd);
        judgementProcessor.AddResult(result);

        if (isHoldEnd)
            hitObject.Data.HoldEndResult = result;
        else
            hitObject.Data.Result = result;
    }

    private void initSnapIndices()
    {
        // shouldn't happen but just in case
        if (Map.TimingPoints == null || Map.TimingPoints.Count == 0) return;

        foreach (var hitObject in Map.HitObjects)
        {
            var time = (int)hitObject.Time;
            var endTime = (int)hitObject.EndTime;

            if (!snapIndices.ContainsKey(time))
                snapIndices.Add(time, getIndex(time));
            if (!snapIndices.ContainsKey(endTime))
                snapIndices.Add(endTime, getIndex(endTime));
        }

        int getIndex(int time)
        {
            var tp = Map.GetTimingPoint(time);
            var diff = time - tp.Time;
            var idx = Math.Round(snaps[0] * diff / tp.MsPerBeat, MidpointRounding.AwayFromZero);

            for (var i = 0; i < snaps.Length; i++)
            {
                if (idx % snaps[i] == 0)
                    return i;
            }

            return snaps.Length - 1;
        }
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
