using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Mods;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class HitObjectManager : Container<HitObject>
{
    [Resolved]
    private AudioClock clock { get; set; }

    public int InternalChildCount => InternalChildren.Count;

    private Bindable<bool> useSnapColors;
    public bool UseSnapColors => useSnapColors.Value;
    private Bindable<float> scrollSpeed;
    public float ScrollSpeed => scrollSpeed.Value;
    public Playfield Playfield { get; }
    public MapInfo Map { get; private set; }
    public List<HitObjectInfo> FutureHitObjects { get; } = new();
    public List<HitObject> HitObjects { get; } = new();
    private Performance performance { get; }

    public double CurrentTime { get; private set; }
    public int CurrentKeyCount { get; private set; }
    public LaneSwitchEvent CurrentLaneSwitchEvent { get; private set; }

    public bool Dead { get; set; }
    private bool skipNextHitSounds { get; set; }

    public HealthMode HealthMode
    {
        get
        {
            if (Playfield.Screen.Mods.Any(m => m is HardMod)) return HealthMode.Drain;

            return Playfield.Screen.Mods.Any(m => m is EasyMod) ? HealthMode.Requirement : HealthMode.Normal;
        }
    }

    public float Health { get; private set; }
    public float HealthDrainRate { get; private set; }

    private List<float> scrollVelocityMarks { get; } = new();
    private Dictionary<int, int> snapIndicies { get; } = new();

    public bool IsFinished => FutureHitObjects.Count == 0 && HitObjects.Count == 0;
    public bool AutoPlay => Playfield.Screen.Mods.Any(m => m is AutoPlayMod);

    public bool Break => timeUntilNextHitObject >= 2000;
    private double timeUntilNextHitObject => (nextHitObject?.Time ?? double.MaxValue) - clock.CurrentTime;
    private double maxHitObjectTime => PositionFromTime(clock.CurrentTime) + 2000 * Playfield.Screen.Rate / ScrollSpeed;

    private HitObjectInfo nextHitObject
    {
        get
        {
            if (HitObjects.Count > 0)
                return HitObjects[0].Data;

            return FutureHitObjects.Count > 0 ? FutureHitObjects[0] : null;
        }
    }

    public HitObjectManager(Playfield playfield)
    {
        Playfield = playfield;
        performance = playfield.Screen.Performance;
        Health = HealthMode == HealthMode.Requirement ? 0 : 100;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1, 1);

        scrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);
        useSnapColors = config.GetBindable<bool>(FluXisSetting.SnapColoring);
        Playfield.Screen.OnSeek += onSeek;
    }

    private void onSeek(double prevTime, double newTime)
    {
        if (!AutoPlay) return;

        newTime = Math.Max(newTime, 0);

        skipNextHitSounds = true;
        clock.Seek(newTime);

        if (newTime < prevTime)
        {
            var hitObjects = Map.HitObjects.Where(h => h.Time >= newTime && h.Time < prevTime).ToList();

            int count = hitObjects.Sum(h => h.IsLongNote() ? 2 : 1);
            performance.Combo -= count;
            performance.Judgements[Judgement.Flawless] -= count;
            performance.HitStats.RemoveAll(h => h.Time > newTime);

            foreach (var hit in hitObjects)
                FutureHitObjects.Add(hit);

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
                performance.AddHitStat(new HitStat(info.Time, 0, Judgement.Flawless));
                performance.AddJudgement(Judgement.Flawless);
                performance.IncCombo();

                if (info.IsLongNote())
                {
                    performance.AddHitStat(new HitStat(info.HoldEndTime, 0, Judgement.Flawless));
                    performance.AddJudgement(Judgement.Flawless);
                    performance.IncCombo();
                }
            }

            FutureHitObjects.RemoveAll(h => h.Time <= newTime);
        }
    }

    protected override void Update()
    {
        updateTime();

        while (FutureHitObjects is { Count: > 0 } && PositionFromTime(FutureHitObjects[0].Time) <= maxHitObjectTime)
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
                    miss(hitObject);
                }

                if (hitObject.IsOffScreen()) removeHitObject(hitObject);
            }
            else miss(hitObject);
        }

        foreach (var laneSwitchEvent in Playfield.Screen.MapEvents.LaneSwitchEvents)
        {
            if (laneSwitchEvent.Time <= clock.CurrentTime)
            {
                CurrentKeyCount = laneSwitchEvent.Count;
                CurrentLaneSwitchEvent = laneSwitchEvent;
            }
        }

        if (!Playfield.Screen.IsPaused.Value && !Break && HealthMode == HealthMode.Drain)
        {
            HealthDrainRate = Math.Max(HealthDrainRate, -1f);

            Health -= HealthDrainRate * ((float)Clock.ElapsedFrameTime / 1000f);
            HealthDrainRate += 0.001f * (float)Clock.ElapsedFrameTime;
        }

        Health = Math.Clamp(Health, 0, 100);
        if (Health == 0 && !Dead && HealthMode != HealthMode.Requirement && !Playfield.Screen.Mods.Any(m => m is NoFailMod))
            Playfield.Screen.Die();

        base.Update();
    }

    private void updateTime()
    {
        int curSv = 0;

        while (Map.ScrollVelocities != null && curSv < Map.ScrollVelocities.Count && Map.ScrollVelocities[curSv].Time <= clock.CurrentTime)
            curSv++;

        CurrentTime = PositionFromTime(clock.CurrentTime, curSv);
    }

    private void updateAutoPlay()
    {
        bool[] pressed = new bool[Map.KeyCount];

        List<HitObject> belowTime = HitObjects.Where(h => h.Data.Time <= clock.CurrentTime).ToList();

        foreach (var hitObject in belowTime.Where(h => !h.GotHit).ToList())
        {
            if (!skipNextHitSounds)
                Playfield.Screen.HitSound.Play();

            hit(hitObject, false);
            pressed[hitObject.Data.Lane - 1] = true;
        }

        foreach (var hitObject in belowTime.Where(h => h.Data.IsLongNote()).ToList())
        {
            hitObject.IsBeingHeld = true;
            pressed[hitObject.Data.Lane - 1] = true;

            if (hitObject.Data.HoldEndTime <= clock.CurrentTime)
                hit(hitObject, true);
        }

        for (var i = 0; i < pressed.Length; i++)
            Playfield.Receptors[i].IsDown = pressed[i];

        skipNextHitSounds = false;
    }

    private void updateInput()
    {
        if (Dead)
            return;

        GameplayInput input = Playfield.Screen.Input;

        if (input.JustPressed.Contains(true))
        {
            Playfield.Screen.HitSound.Play();

            List<HitObject> hitable = HitObjects.Where(hit => hit.Hitable && input.JustPressed[hit.Data.Lane - 1]).ToList();

            bool[] pressed = new bool[Map.KeyCount];

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

            bool[] pressed = new bool[Map.KeyCount];

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
        double diff = isHoldEnd ? hitObject.Data.HoldEndTime - clock.CurrentTime : hitObject.Data.Time - clock.CurrentTime;
        diff = AutoPlay ? 0 : diff;
        diff /= clock.Rate;
        hitObject.GotHit = true;

        judmentDisplay(hitObject, diff);

        performance.IncCombo();

        if (!hitObject.Data.IsLongNote() || isHoldEnd) removeHitObject(hitObject);
    }

    private void miss(HitObject hitObject)
    {
        if (performance.Combo >= 5)
            Playfield.Screen.Combobreak.Play();

        judmentDisplay(hitObject, 0, !AutoPlay);
        performance.ResetCombo();

        if (!hitObject.Data.IsLongNote()) removeHitObject(hitObject);
    }

    private void removeHitObject(HitObject hitObject)
    {
        HitObjects.Remove(hitObject);

        RemoveInternal(hitObject, false);
    }

    private void judmentDisplay(HitObject hitObject, double difference, bool missed = false)
    {
        HitWindow hitWindow = missed ? HitWindow.FromKey(Judgement.Miss) : HitWindow.FromTiming((float)Math.Abs(difference));

        if (Dead)
            return;

        performance.AddHitStat(new HitStat(hitObject.Data.Time, (float)difference, hitWindow.Key));
        performance.AddJudgement(hitWindow.Key);

        if (HealthMode == HealthMode.Drain)
            HealthDrainRate -= hitWindow.DrainRate;
        else
            Health += hitWindow.Health;

        if (hitWindow.Key == Judgement.Miss && Playfield.Screen.Mods.Any(m => m is FragileMod))
            Playfield.Screen.Die();

        if (hitWindow.Key != Judgement.Flawless && Playfield.Screen.Mods.Any(m => m is FlawlessMod))
            Playfield.Screen.Die();
    }

    public void LoadMap(MapInfo map)
    {
        Map = map;
        CurrentKeyCount = map.InitialKeyCount;
        initScrollVelocityMarks();
        initSnapIndicies();

        foreach (var hit in map.HitObjects)
        {
            if (Playfield.Screen.Mods.Any(m => m is NoLnMod))
                hit.HoldTime = 0;

            FutureHitObjects.Add(hit);
        }
    }

    private void initScrollVelocityMarks()
    {
        if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0 || Playfield.Screen.Mods.Any(m => m is NoSvMod))
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

    public double PositionFromTime(double time, int index = -1)
    {
        if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0 || Playfield.Screen.Mods.Any(m => m is NoSvMod))
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
}
