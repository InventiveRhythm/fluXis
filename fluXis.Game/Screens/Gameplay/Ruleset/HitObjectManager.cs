using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Mods;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Scoring.Structs;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class HitObjectManager : Container<HitObject>
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    private Bindable<bool> useSnapColors;
    public bool UseSnapColors => useSnapColors.Value;

    private Bindable<float> scrollSpeed;
    public float ScrollSpeed => scrollSpeed.Value * (scrollSpeed.Value / (scrollSpeed.Value * screen.Rate));

    public MapInfo Map => playfield.Map;
    public int KeyCount => playfield.RealmMap.KeyCount;
    public List<HitObjectInfo> PastHitObjects { get; } = new();
    public List<HitObjectInfo> FutureHitObjects { get; } = new();
    public List<HitObject> HitObjects { get; } = new();

    public double CurrentTime { get; private set; }
    public int CurrentKeyCount { get; private set; }
    public LaneSwitchEvent CurrentLaneSwitchEvent { get; private set; }

    private bool skipNextHitSounds { get; set; }

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

    public bool AutoPlay => screen.Mods.Any(m => m is AutoPlayMod);

    public bool Break => timeUntilNextHitObject >= 2000;
    private double timeUntilNextHitObject => (nextHitObject?.Time ?? double.MaxValue) - Clock.CurrentTime;

    private HitObjectInfo nextHitObject
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
        screen.OnSeek += onSeek;
    }

    private void onSeek(double prevTime, double newTime)
    {
        if (!AutoPlay) return;

        newTime = Math.Max(newTime, 0);

        skipNextHitSounds = true;
        (Clock as AudioClock)?.Seek(newTime);

        if (newTime < prevTime)
        {
            var hitObjects = PastHitObjects.Where(h => h.Time >= newTime).ToList();

            foreach (var hitObject in hitObjects)
            {
                if (hitObject.IsLongNote() && hitObject.HoldEndTime >= newTime && hitObject.HoldEndResult != null)
                {
                    var endResult = hitObject.HoldEndResult;
                    judgementProcessor.RevertResult(endResult);
                    hitObject.HoldEndResult = null;
                }

                if (hitObject.Result == null) continue;

                var result = hitObject.Result;
                judgementProcessor.RevertResult(result);
                hitObject.Result = null;

                var hit = new HitObject(this, hitObject);
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

                if (info.IsLongNote())
                {
                    var endHitResult = new HitResult(info.HoldEndTime, 0, Judgement.Flawless);
                    judgementProcessor.AddResult(endHitResult);
                    info.HoldEndResult = endHitResult;
                }
            }

            var toPast = FutureHitObjects.Where(h => h.Time <= newTime);
            PastHitObjects.AddRange(toPast);
            FutureHitObjects.RemoveAll(h => h.Time <= newTime);
        }
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
            HitObject hitObject = new HitObject(this, FutureHitObjects[0]);
            FutureHitObjects.RemoveAt(0);
            HitObjects.Add(hitObject);
            AddInternal(hitObject);
        }

        if (AutoPlay)
            updateAutoPlay();
        else
            updateInput();

        foreach (var hitObject in HitObjects.Where(h => h.Missed).ToList())
        {
            if (hitObject.Data.IsLongNote())
            {
                if (!hitObject.LongNoteMissed)
                {
                    hitObject.MissLongNote();
                    miss(hitObject, hitObject.GotHit);
                }

                if (hitObject.Data.HoldEndTime - Clock.CurrentTime <= screen.ReleaseWindows.TimingFor(screen.ReleaseWindows.Lowest))
                    removeHitObject(hitObject);
            }
            else miss(hitObject);
        }

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

    private void updateTime()
    {
        int svIndex = 0;

        while (Map.ScrollVelocities != null && svIndex < Map.ScrollVelocities.Count && Map.ScrollVelocities[svIndex].Time <= Clock.CurrentTime)
            svIndex++;

        CurrentTime = ScrollVelocityPositionFromTime(Clock.CurrentTime, svIndex);
    }

    private void updateAutoPlay()
    {
        bool[] pressed = new bool[KeyCount];

        List<HitObject> belowTime = HitObjects.Where(h => h.Data.Time <= Clock.CurrentTime).ToList();

        foreach (var hitObject in belowTime.Where(h => !h.GotHit).ToList())
        {
            if (!skipNextHitSounds)
            {
                var sample = screen.Hitsounding.GetSample(hitObject.Data.HitSound) ?? screen.Samples.HitSample;
                sample?.Play();
            }

            hit(hitObject, false);
            pressed[hitObject.Data.Lane - 1] = true;
        }

        foreach (var hitObject in belowTime.Where(h => h.Data.IsLongNote()).ToList())
        {
            hitObject.IsBeingHeld = true;
            pressed[hitObject.Data.Lane - 1] = true;

            if (hitObject.Data.HoldEndTime <= Clock.CurrentTime)
                hit(hitObject, true);
        }

        for (var i = 0; i < pressed.Length; i++)
            playfield.Receptors[i].IsDown = pressed[i];

        skipNextHitSounds = false;
    }

    private void updateInput()
    {
        if (screen.HealthProcessor.Failed)
            return;

        GameplayInput input = screen.Input;

        if (input.JustPressed.Contains(true))
        {
            for (var i = 0; i < input.JustPressed.Length; i++)
            {
                if (!input.JustPressed[i]) continue;

                var next = nextInLane(i + 1);
                if (next == null) continue;

                var sample = screen.Hitsounding.GetSample(next.HitSound) ?? screen.Samples.HitSample;
                sample?.Play();
            }

            List<HitObject> hitable = HitObjects.Where(hit => hit.Hitable && input.JustPressed[hit.Data.Lane - 1]).ToList();

            bool[] pressed = new bool[KeyCount];

            if (hitable.Count > 0)
            {
                foreach (var hitObject in hitable)
                {
                    if (!pressed[hitObject.Data.Lane - 1])
                    {
                        hit(hitObject, false);
                        pressed[hitObject.Data.Lane - 1] = true;
                    }
                }
            }
        }

        if (input.Pressed.Contains(true))
        {
            foreach (var hit in HitObjects)
            {
                if (hit.Hitable && hit.GotHit && hit.Data.IsLongNote() && input.Pressed[hit.Data.Lane - 1])
                    hit.IsBeingHeld = true;
            }
        }

        if (input.JustReleased.Contains(true))
        {
            List<HitObject> releaseable = HitObjects.Where(hit => hit.Releasable && input.JustReleased[hit.Data.Lane - 1]).ToList();

            bool[] pressed = new bool[KeyCount];

            if (releaseable.Count > 0)
            {
                foreach (var hitObject in releaseable)
                {
                    if (!pressed[hitObject.Data.Lane - 1])
                    {
                        hit(hitObject, true);
                        pressed[hitObject.Data.Lane - 1] = true;
                    }
                }
            }
        }
    }

    private void hit(HitObject hitObject, bool isHoldEnd)
    {
        double diff = isHoldEnd ? hitObject.Data.HoldEndTime - Clock.CurrentTime : hitObject.Data.Time - Clock.CurrentTime;
        diff = AutoPlay ? 0 : diff;
        hitObject.GotHit = true;

        judmentDisplay(hitObject, diff, isHoldEnd);

        if (!hitObject.Data.IsLongNote() || isHoldEnd) removeHitObject(hitObject);
    }

    private void miss(HitObject hitObject, bool isHoldEnd = false)
    {
        var windows = isHoldEnd ? screen.ReleaseWindows : screen.HitWindows;
        judmentDisplay(hitObject, -windows.TimingFor(windows.Lowest), isHoldEnd);

        if (!hitObject.Data.IsLongNote()) removeHitObject(hitObject);
    }

    private void removeHitObject(HitObject hitObject)
    {
        HitObjects.Remove(hitObject);
        PastHitObjects.Add(hitObject.Data);
        RemoveInternal(hitObject, false);
    }

    private void judmentDisplay(HitObject hitObject, double difference, bool isHoldEnd = false)
    {
        var hitWindows = isHoldEnd ? screen.ReleaseWindows : screen.HitWindows;
        var judgement = hitWindows.JudgementFor(difference);

        if (screen.HealthProcessor.Failed)
            return;

        var result = new HitResult(hitObject.Data.Time, (float)difference, judgement);
        judgementProcessor.AddResult(result);

        if (isHoldEnd)
            hitObject.Data.HoldEndResult = result;
        else
            hitObject.Data.Result = result;

        if (judgement == hitWindows.ComboBreakJudgement && screen.Mods.Any(m => m is FragileMod))
            screen.OnDeath();

        if (judgement != hitWindows.HighestHitable && screen.Mods.Any(m => m is FlawlessMod))
            screen.OnDeath();
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

        ScrollVelocityInfo first = Map.ScrollVelocities[0];

        float time = first.Time;
        scrollVelocityMarks.Add(time);

        for (var i = 1; i < Map.ScrollVelocities.Count; i++)
        {
            ScrollVelocityInfo prev = Map.ScrollVelocities[i - 1];
            ScrollVelocityInfo current = Map.ScrollVelocities[i];

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
            TimingPointInfo timingPoint = Map.TimingPoints[i];
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

        ScrollVelocityInfo prev = Map.ScrollVelocities[index - 1];

        double position = scrollVelocityMarks[index - 1];
        position += (time - prev.Time) * prev.Multiplier;
        return position;
    }

    [CanBeNull]
    private HitObjectInfo nextInLane(int lane)
    {
        var hit = HitObjects.FirstOrDefault(h => h.Data.Lane == lane && !h.GotHit)?.Data;
        hit ??= FutureHitObjects.FirstOrDefault(h => h.Lane == lane);
        return hit;
    }
}
