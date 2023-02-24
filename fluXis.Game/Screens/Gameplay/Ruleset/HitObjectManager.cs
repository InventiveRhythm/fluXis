using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset
{
    public partial class HitObjectManager : CompositeDrawable
    {
        private Bindable<float> scrollSpeed;
        public float ScrollSpeed => scrollSpeed.Value;
        public Playfield Playfield { get; }
        public Performance Performance { get; }
        public MapInfo Map { get; set; }
        public List<HitObject> FutureHitObjects { get; } = new();
        public List<HitObject> HitObjects { get; } = new();

        public float CurrentTime { get; private set; }
        public int CurrentKeyCount { get; private set; }

        public bool Dead { get; set; }
        public HealthMode HealthMode => HealthMode.Normal;
        public float Health { get; private set; }
        public float HealthDrainRate { get; private set; }

        public List<float> ScrollVelocityMarks { get; } = new();

        public bool IsFinished => FutureHitObjects.Count == 0 && HitObjects.Count == 0;

        public Bindable<bool> AutoPlay = new();

        public HitObjectManager(Playfield playfield)
        {
            Playfield = playfield;
            Performance = playfield.Screen.Performance;
            Health = HealthMode == HealthMode.Requirement ? 0 : 100;
        }

        [BackgroundDependencyLoader]
        private void load(FluXisConfig config)
        {
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(1, 1);

            scrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);
        }

        protected override void Update()
        {
            updateTime();

            if (AutoPlay.Value)
                updateAutoPlay();
            else
                updateInput();

            while (FutureHitObjects != null && FutureHitObjects.Count > 0 && FutureHitObjects[0].ScrollVelocityTime <= CurrentTime + 2000 * ScrollSpeed)
            {
                HitObject hitObject = FutureHitObjects[0];
                FutureHitObjects.RemoveAt(0);
                HitObjects.Add(hitObject);
                AddInternal(hitObject);
            }

            foreach (var hitObject in HitObjects.Where(h => h.Missed && h.Exists).ToList())
            {
                if (hitObject.Data.IsLongNote())
                {
                    if (!hitObject.LongNoteMissed)
                    {
                        hitObject.MissLongNote();
                        miss(hitObject);
                    }

                    if (hitObject.IsOffScreen())
                    {
                        hitObject.Kill();
                        HitObjects.Remove(hitObject);
                        RemoveInternal(hitObject, true);
                    }
                }
                else
                {
                    miss(hitObject);
                }
            }

            foreach (var eventInfo in Map.Events)
            {
                if (eventInfo.Type == "laneswitch")
                {
                    if (eventInfo.Time <= CurrentTime)
                    {
                        CurrentKeyCount = eventInfo.Value;
                    }
                }
            }

            if (!Playfield.Screen.IsPaused.Value && HitObjects.Count > 0 && HitObjects[0].Data.Time - 4000 < Conductor.CurrentTime && HealthMode == HealthMode.Drain)
            {
                HealthDrainRate = Math.Max(HealthDrainRate, -1f);

                Health -= HealthDrainRate * ((float)Clock.ElapsedFrameTime / 1000f);
                HealthDrainRate += 0.001f * (float)Clock.ElapsedFrameTime;
            }

            Health = Math.Clamp(Health, 0, 100);
            if (Health == 0 && !Dead && HealthMode != HealthMode.Requirement)
                Playfield.Screen.Die();

            base.Update();
        }

        private void updateTime()
        {
            int curSv = 0;

            while (Map.ScrollVelocities != null && curSv < Map.ScrollVelocities.Count && Map.ScrollVelocities[curSv].Time <= Conductor.CurrentTime)
                curSv++;

            CurrentTime = PositionFromTime(Conductor.CurrentTime, curSv);
        }

        private void updateAutoPlay()
        {
            Playfield.Screen.CanSubmitScore = false;

            List<HitObject> belowTime = HitObjects.Where(h => h.Data.Time <= Conductor.CurrentTime && h.Exists).ToList();

            foreach (var hitObject in belowTime.Where(h => !h.GotHit).ToList())
            {
                Playfield.Screen.HitSound.Play();
                hit(hitObject, false);
            }

            foreach (var hitObject in belowTime.Where(h => h.Data.IsLongNote()).ToList())
            {
                hitObject.IsBeingHeld = true;

                if (hitObject.Data.HoldEndTime <= Conductor.CurrentTime)
                {
                    hit(hitObject, true);
                }
            }
        }

        private void updateInput()
        {
            if (Dead)
                return;

            GameplayInput input = Playfield.Screen.Input;

            if (input.JustPressed.Contains(true))
            {
                Playfield.Screen.HitSound.Play();

                List<HitObject> hitable = new List<HitObject>();

                foreach (var hit in HitObjects)
                {
                    if (hit.Hitable && input.JustPressed[hit.Data.Lane - 1])
                        hitable.Add(hit);
                }

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
                List<HitObject> releaseable = new List<HitObject>();

                foreach (var hit in HitObjects)
                {
                    if (hit.Releasable && input.JustReleased[hit.Data.Lane - 1])
                        releaseable.Add(hit);
                }

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
            float diff = isHoldEnd ? hitObject.Data.HoldEndTime - Conductor.CurrentTime : hitObject.Data.Time - Conductor.CurrentTime;
            hitObject.GotHit = true;

            judmentDisplay(hitObject, diff);
            Playfield.Screen.HitErrorBar.AddHit(diff);

            Performance.IncCombo();

            if (hitObject.Data.IsLongNote() && !isHoldEnd) { }
            else
            {
                hitObject.Kill();
                HitObjects.Remove(hitObject);
                RemoveInternal(hitObject, true);
            }
        }

        private void miss(HitObject hitObject)
        {
            if (Performance.Combo >= 5)
                Playfield.Screen.Combobreak.Play();

            judmentDisplay(hitObject, 0, true);
            Performance.ResetCombo();

            if (hitObject.Data.IsLongNote()) return;

            hitObject.Kill();
            HitObjects.Remove(hitObject);
            RemoveInternal(hitObject, true);
        }

        private void judmentDisplay(HitObject hitObject, float difference, bool missed = false)
        {
            HitWindow hitWindow = missed ? HitWindow.FromKey(Judgements.Miss) : HitWindow.FromTiming(Math.Abs(difference));

            Playfield.Screen.JudgementDisplay.PopUp(hitWindow);

            if (Dead)
                return;

            Performance.AddHitPoint(new HitPoint(hitObject.Data.Time, difference, hitWindow.Key));
            Performance.AddJudgement(hitWindow.Key);

            if (HealthMode == HealthMode.Drain)
                HealthDrainRate -= hitWindow.Health;
            else
                Health += hitWindow.Health;
        }

        public void LoadMap(MapInfo map)
        {
            Map = map;
            CurrentKeyCount = map.InitialKeyCount;
            initScrollVelocityMarks();

            foreach (var hit in map.HitObjects)
            {
                HitObject hitObject = new HitObject(this, hit);
                FutureHitObjects.Add(hitObject);
            }

            // Conductor.Offset = map.HitObjects[0].Time - map.TimingPoints[0].Time;
        }

        private void initScrollVelocityMarks()
        {
            if (Map.ScrollVelocities == null || Map.ScrollVelocities.Count == 0)
                return;

            ScrollVelocityInfo first = Map.ScrollVelocities[0];

            float time = first.Time;
            ScrollVelocityMarks.Add(time);

            for (var i = 1; i < Map.ScrollVelocities.Count; i++)
            {
                ScrollVelocityInfo prev = Map.ScrollVelocities[i - 1];
                ScrollVelocityInfo current = Map.ScrollVelocities[i];

                time += (int)((current.Time - prev.Time) * prev.Multiplier);
                ScrollVelocityMarks.Add(time);
            }
        }

        public float PositionFromTime(float time, int index = -1)
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

            ScrollVelocityInfo prev = Map.ScrollVelocities[index - 1];

            float position = ScrollVelocityMarks[index - 1];
            position += (time - prev.Time) * prev.Multiplier;
            return position;
        }
    }
}
