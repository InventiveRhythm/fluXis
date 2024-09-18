using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures;
using fluXis.Game.Mods;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Skinning;
using fluXis.Shared.Scoring.Structs;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

public partial class HitObjectManager : Container<DrawableHitObject>
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    private GameplayInput input => screen.Input;

    private Bindable<bool> useSnapColors;
    public bool UseSnapColors => useSnapColors.Value;

    private Bindable<float> scrollSpeed;
    public float ScrollSpeed => scrollSpeed.Value * (scrollSpeed.Value / (scrollSpeed.Value * screen.Rate));

    private Bindable<bool> hitsounds;

    public MapInfo Map => playfield.Map;
    public int KeyCount => playfield.RealmMap.KeyCount;
    public Stack<HitObject> PastHitObjects { get; } = new();
    public List<HitObject> FutureHitObjects { get; } = new();
    public List<DrawableHitObject> HitObjects { get; } = new();

    public double CurrentTime { get; private set; }

    public bool Seeking { get; private set; }

    public HealthMode HealthMode
    {
        get
        {
            if (screen.Mods.Any(m => m is HardMod)) return HealthMode.Drain;

            return screen.Mods.Any(m => m is EasyMod) ? HealthMode.Requirement : HealthMode.Normal;
        }
    }

    private JudgementProcessor judgementProcessor => screen.JudgementProcessor;

    private List<double> scrollVelocityMarks { get; } = new();

    private static int[] snaps { get; } = { 48, 24, 16, 12, 8, 6, 4, 3 };
    private Dictionary<int, int> snapIndices { get; } = new();

    public Action OnFinished { get; set; }
    public bool Finished { get; private set; }

    public bool Break => timeUntilNextHitObject >= 2000;
    private double timeUntilNextHitObject => (nextHitObject?.Time ?? double.MaxValue) - Clock.CurrentTime;

    private HitObject nextHitObject
    {
        get
        {
            if (HitObjects.Count > 0)
                return HitObjects[0].Data;

            return FutureHitObjects.Count > 0 ? FutureHitObjects[0] : null;
        }
    }

    private const float minimum_loaded_hit_objects = 10;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;

        loadMap();

        scrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);
        useSnapColors = config.GetBindable<bool>(FluXisSetting.SnapColoring);
        hitsounds = config.GetBindable<bool>(FluXisSetting.Hitsounding);
        screen.OnSeek += onSeek;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        input.OnPress += key =>
        {
            var lane = input.Keys.IndexOf(key) + 1;
            var hit = nextInLane(lane);

            if (hit == null)
                return;

            PlayHitSound(hit);
        };
    }

    private void onSeek(double prevTime, double newTime)
    {
        newTime = Math.Max(newTime, 0);

        /*Seeking = true;
        (Clock as GameplayClock)?.Seek(newTime);

        if (newTime < prevTime)
        {
            var hitObjects = PastHitObjects.Where(h => h.Time >= newTime).ToList();

            foreach (var hitObject in hitObjects)
            {
                if (hitObject.LongNote && hitObject.EndTime >= newTime && hitObject.HoldEndResult != null)
                {
                    var endResult = hitObject.HoldEndResult;
                    judgementProcessor.RevertResult(endResult);
                    hitObject.HoldEndResult = null;
                }

                if (hitObject.Result == null) continue;

                var result = hitObject.Result;
                judgementProcessor.RevertResult(result);
                hitObject.Result = null;

                var hit = new OldDrawableHitObject(this, hitObject);
                HitObjects.Add(hit);
                AddInternal(hit);
            }

            PastHitObjects.RemoveAll(h => h.Time >= newTime);
            FutureHitObjects.RemoveAll(h => h.Time <= newTime);
            FutureHitObjects.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
        else
        {
            // all future hitobjects behind the new time
            var hitObjects = FutureHitObjects.Where(h => h.Time <= newTime).ToList();

            // remove all hitobjects behind the new time
            foreach (var info in hitObjects)
            {
                var hitResult = new HitResult(info.Time, 0, Judgement.Flawless);
                judgementProcessor.AddResult(hitResult);
                info.Result = hitResult;

                if (info.LongNote)
                {
                    var endHitResult = new HitResult(info.EndTime, 0, Judgement.Flawless);
                    judgementProcessor.AddResult(endHitResult);
                    info.HoldEndResult = endHitResult;
                }
            }

            var toPast = FutureHitObjects.Where(h => h.Time <= newTime);
            PastHitObjects.AddRange(toPast);
            FutureHitObjects.RemoveAll(h => h.Time <= newTime);
        }*/
    }

    protected override void Update()
    {
        updateTime();

        if (HitObjects.Count == 0 && FutureHitObjects.Count == 0 && !Finished)
        {
            Finished = true;
            OnFinished?.Invoke();
        }

        while (FutureHitObjects is { Count: > 0 } && (ShouldDisplay(FutureHitObjects[0].Time) || HitObjects.Count < minimum_loaded_hit_objects))
        {
            var hit = createHitObject(FutureHitObjects[0]);
            HitObjects.Add(hit);
            AddInternal(hit);

            FutureHitObjects.RemoveAt(0);
        }

        while (HitObjects.Count > 0 && !ShouldDisplay(HitObjects.Last().Data.Time) && HitObjects.Count > minimum_loaded_hit_objects)
        {
            var hit = HitObjects.Last();
            removeHitObject(hit, true);
        }

        foreach (var hitObject in HitObjects.Where(h => h.CanBeRemoved).ToList())
            removeHitObject(hitObject);

        while (PastHitObjects.Count > 0)
        {
            var result = PastHitObjects.Peek().Result;

            if (result is null || Clock.CurrentTime >= result.Time)
                break;

            revertHitObject(PastHitObjects.Pop());
        }
    }

    public float HitPosition => DrawHeight - laneSwitchManager.HitPosition;

    public bool ShouldDisplay(double time) => ScrollVelocityPositionFromTime(time) <= ScrollVelocityPositionFromTime(Clock.CurrentTime) + DrawHeight * screen.Rate;
    public float PositionAtTime(double time) => (float)(HitPosition - .5f * ((time - (float)CurrentTime) * ScrollSpeed));

    public float PositionAtLane(int lane)
    {
        var receptors = playfield.Receptors;
        var x = 0f;

        for (int i = 1; i < lane; i++)
        {
            if (i > receptors.Count)
                x += skinManager.SkinJson.GetKeymode(KeyCount).ColumnWidth;
            else
                x += receptors[i - 1].Width;
        }

        return x;
    }

    public float WidthOfLane(int lane) => laneSwitchManager.WidthFor(lane);

    public bool IsFirstInColumn(DrawableHitObject hitObject) => HitObjects.FirstOrDefault(h => h.Data.Lane == hitObject.Data.Lane && h.Data.Time < hitObject.Data.Time) == null;

    private DrawableHitObject createHitObject(HitObject hitObject)
    {
        var drawable = getDrawableFor(hitObject);
        var idx = hitObject.Lane - 1;

        if (screen.Input.Keys.Count > idx)
            drawable.Keybind = screen.Input.Keys[idx];

        drawable.OnHit += hit;

        drawable.OnLoadComplete += _ =>
        {
            for (var i = 0; i < screen.Input.Pressed.Length; i++)
            {
                if (!screen.Input.Pressed[i])
                    continue;

                var bind = screen.Input.Keys[i];
                drawable.OnPressed(bind);
            }
        };

        return drawable;
    }

    private void revertHitObject(HitObject hit)
    {
        if (hit.HoldEndResult is not null)
            judgementProcessor.RevertResult(hit.HoldEndResult);

        judgementProcessor.RevertResult(hit.Result);

        var draw = createHitObject(hit);
        HitObjects.Insert(0, draw);
        AddInternal(draw);
    }

    private void removeHitObject(DrawableHitObject hitObject, bool addToFuture = false)
    {
        if (!addToFuture)
            hitObject.OnKill();

        hitObject.OnHit -= hit;

        HitObjects.Remove(hitObject);

        if (addToFuture)
            FutureHitObjects.Insert(0, hitObject.Data);
        else
            PastHitObjects.Push(hitObject.Data);

        RemoveInternal(hitObject, true);
    }

    private DrawableHitObject getDrawableFor(HitObject hit)
    {
        switch (hit.Type)
        {
            case 1:
                return new DrawableTickNote(hit);

            default:
            {
                if (hit.LongNote)
                    return new DrawableLongNote(hit);

                return new DrawableNote(hit);
            }
        }
    }

    private void updateTime()
    {
        int svIndex = 0;

        while (Map.ScrollVelocities != null && svIndex < Map.ScrollVelocities.Count && Map.ScrollVelocities[svIndex].Time <= Clock.CurrentTime)
            svIndex++;

        CurrentTime = ScrollVelocityPositionFromTime(Clock.CurrentTime, svIndex);
    }

    private void hit(DrawableHitObject hitObject, double difference)
    {
        // since judged is only set after hitting the tail this works
        var isHoldEnd = hitObject is DrawableLongNote { Judged: true };

        var hitWindows = isHoldEnd ? screen.ReleaseWindows : screen.HitWindows;
        var judgement = hitWindows.JudgementFor(difference);

        if (screen.HealthProcessor.Failed)
            return;

        var result = new HitResult(Time.Current, difference, judgement);
        judgementProcessor.AddResult(result);

        if (isHoldEnd)
            hitObject.Data.HoldEndResult = result;
        else
            hitObject.Data.Result = result;
    }

    public void PlayHitSound(HitObject hitObject, bool userTriggered = true)
    {
        // ignore hitsounds when the next is a
        // tick note since it would be played twice
        // when hitting them as a normal note
        if (hitObject is { Type: 1 } && userTriggered) return;

        var channel = screen.Hitsounding.GetSample(hitObject.HitSound, hitsounds.Value);
        channel?.Play();
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

    private void loadMap()
    {
        initScrollVelocityMarks();
        initSnapIndices();

        Map.HitObjects.ForEach(FutureHitObjects.Add);
    }

    private void initScrollVelocityMarks()
    {
        if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0)
            return;

        ScrollVelocity first = Map.ScrollVelocities[0];

        var time = first.Time;
        scrollVelocityMarks.Add(time);

        for (var i = 1; i < Map.ScrollVelocities.Count; i++)
        {
            ScrollVelocity prev = Map.ScrollVelocities[i - 1];
            ScrollVelocity current = Map.ScrollVelocities[i];

            time += (int)((current.Time - prev.Time) * prev.Multiplier);
            scrollVelocityMarks.Add(time);
        }
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

    public double ScrollVelocityPositionFromTime(double time, int index = -1)
    {
        if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0)
            return time;

        if (index == -1)
        {
            for (index = 0; index < Map.ScrollVelocities.Count; index++)
            {
                if (time < Map.ScrollVelocities[index].Time)
                    break;
            }
        }

        if (index == 0)
            return time;

        ScrollVelocity prev = Map.ScrollVelocities[index - 1];

        double position = scrollVelocityMarks[index - 1];
        position += (time - prev.Time) * prev.Multiplier;
        return position;
    }

    [CanBeNull]
    private HitObject nextInLane(int lane)
    {
        var hit = HitObjects.FirstOrDefault(h => h.Data.Lane == lane && !h.Judged)?.Data;
        hit ??= FutureHitObjects.FirstOrDefault(h => h.Lane == lane);
        return hit;
    }
}
