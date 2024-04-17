using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
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

    private GameplayInput input => screen.Input;

    private Bindable<bool> useSnapColors;
    public bool UseSnapColors => useSnapColors.Value;

    private Bindable<float> scrollSpeed;
    public float ScrollSpeed => scrollSpeed.Value * (scrollSpeed.Value / (scrollSpeed.Value * screen.Rate));

    private Bindable<bool> hitsounds;

    public MapInfo Map => playfield.Map;
    public int KeyCount => playfield.RealmMap.KeyCount;
    public List<HitObject> PastHitObjects { get; } = new();
    public List<HitObject> FutureHitObjects { get; } = new();
    public List<DrawableHitObject> HitObjects { get; } = new();

    public double CurrentTime { get; private set; }
    public int CurrentKeyCount { get; private set; }
    public LaneSwitchEvent CurrentLaneSwitchEvent { get; private set; }

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

    private List<float> scrollVelocityMarks { get; } = new();
    private Dictionary<int, int> snapIndicies { get; } = new();

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

        while (FutureHitObjects is { Count: > 0 } && ShouldDisplay(FutureHitObjects[0].Time))
        {
            var hit = createHitObject(FutureHitObjects[0]);
            HitObjects.Add(hit);
            AddInternal(hit);

            FutureHitObjects.RemoveAt(0);
        }

        while (HitObjects.Count > 0 && !ShouldDisplay(HitObjects.Last().Data.Time))
        {
            var hit = HitObjects.Last();
            removeHitObject(hit, true);
        }

        foreach (var hitObject in HitObjects.Where(h => h.CanBeRemoved).ToList())
            removeHitObject(hitObject);

        foreach (var laneSwitchEvent in screen.MapEvents.LaneSwitchEvents)
        {
            if (laneSwitchEvent.Time <= Clock.CurrentTime)
            {
                CurrentKeyCount = laneSwitchEvent.Count;
                CurrentLaneSwitchEvent = laneSwitchEvent;
            }
        }

        base.Update();
    }

    public float HitPosition => DrawHeight - skinManager.SkinJson.GetKeymode(CurrentKeyCount).HitPosition;

    public bool ShouldDisplay(float time) => ScrollVelocityPositionFromTime(time) <= ScrollVelocityPositionFromTime(Clock.CurrentTime) + DrawHeight * screen.Rate;
    public float PositionAtTime(double time) => PositionAtTime((float)time);
    public float PositionAtTime(float time) => HitPosition - .5f * ((time - (float)CurrentTime) * ScrollSpeed);

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

    public float WidthOfLane(int lane)
    {
        var receptors = playfield.Receptors;

        if (lane > receptors.Count)
            return skinManager.SkinJson.GetKeymode(KeyCount).ColumnWidth;

        return receptors[lane - 1].Width;
    }

    public bool IsFirstInColumn(DrawableHitObject hitObject) => HitObjects.FirstOrDefault(h => h.Data.Lane == hitObject.Data.Lane && h.Data.Time < hitObject.Data.Time) == null;

    private DrawableHitObject createHitObject(HitObject hitObject)
    {
        var drawable = getDrawableFor(hitObject);
        drawable.Keybind = screen.Input.Keys[hitObject.Lane - 1];
        drawable.OnHit += hit;

        drawable.OnLoadComplete += _ =>
        {
            for (var i = 0; i < screen.Input.Pressed.Length; i++)
            {
                var bind = screen.Input.Keys[i];
                drawable.OnPressed(bind);
            }
        };

        return drawable;
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
            PastHitObjects.Add(hitObject.Data);

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

    private void hit(DrawableHitObject hitObject, float difference)
    {
        // since judged is only set after hitting the tail this works
        var isHoldEnd = hitObject is DrawableLongNote { Judged: true };

        var hitWindows = isHoldEnd ? screen.ReleaseWindows : screen.HitWindows;
        var judgement = hitWindows.JudgementFor(difference);

        if (screen.HealthProcessor.Failed)
            return;

        var result = new HitResult(hitObject.Data.Time, difference, judgement);
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

    private void loadMap()
    {
        CurrentKeyCount = Map.InitialKeyCount;
        initScrollVelocityMarks();
        initSnapIndicies();

        foreach (var hit in Map.HitObjects)
        {
            if (screen.Mods.Any(m => m is NoLnMod))
                hit.HoldTime = 0;

            FutureHitObjects.Add(hit);
        }
    }

    private void initScrollVelocityMarks()
    {
        if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0 || screen.Mods.Any(m => m is NoSvMod))
            return;

        ScrollVelocity first = Map.ScrollVelocities[0];

        float time = first.Time;
        scrollVelocityMarks.Add(time);

        for (var i = 1; i < Map.ScrollVelocities.Count; i++)
        {
            ScrollVelocity prev = Map.ScrollVelocities[i - 1];
            ScrollVelocity current = Map.ScrollVelocities[i];

            time += (int)((current.Time - prev.Time) * prev.Multiplier);
            scrollVelocityMarks.Add(time);
        }
    }

    private void initSnapIndicies()
    {
        // shouldn't happen but just in case
        if (Map.TimingPoints == null || Map.TimingPoints.Count == 0) return;

        for (var i = 0; i < Map.TimingPoints.Count; i++)
        {
            TimingPoint timingPoint = Map.TimingPoints[i];
            double time = timingPoint.Time;
            float target = i + 1 < Map.TimingPoints.Count ? Map.TimingPoints[i + 1].Time : Map.EndTime;
            float increment = timingPoint.MsPerBeat;

            while (time < target)
            {
                for (int j = 0; j < 16; j++)
                {
                    var add = increment / 16 * j;
                    var snap = time + add;

                    snapIndicies.TryAdd((int)snap, j);
                }

                time += increment;
            }
        }
    }

    public int GetSnapIndex(int time)
    {
        if (snapIndicies.TryGetValue(time, out int i)) return i;

        // allow a 10ms margin of error for snapping
        var closest = snapIndicies.Keys.MinBy(k => Math.Abs(k - time));
        if (Math.Abs(closest - time) <= 10 && snapIndicies.TryGetValue(closest, out i)) return i;

        // still nothing...
        return -1;
    }

    public double ScrollVelocityPositionFromTime(double time, int index = -1)
    {
        if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0 || screen.Mods.Any(m => m is NoSvMod))
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
