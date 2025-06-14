using System;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableTickNote : DrawableHitObject
{
    public override bool CanBeRemoved => Judged || wouldMiss;

    private bool wouldMiss => Time.Current - Data.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    [Resolved]
    private GameplayInput input { get; set; }

    private bool isBeingHeld;
    private double? holdStartTime;
    private bool directHit;

    private Circle followLine;

    public DrawableTickNote(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new[]
        {
            followLine = new Circle()
            {
                BypassAutoSizeAxes = Axes.Both,
                Colour = Colour4.FromHex("#F2C979").Opacity(.4f),
                Size = new Vector2(8),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            Skin.GetTickNote(VisualLane, ObjectManager.KeyCount, Data.HoldTime > 0)
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Data.VisualLane > 1)
            X = ObjectManager.PositionAtLane(Data.VisualLane);

        if (isBeingHeld)
            UpdateJudgement(true);

        var next = Data.NextObject;

        if (next?.Type == 1 && next.Lane == Data.Lane && (Data.VisualLane != 0 || next.VisualLane != 0))
        {
            var l = next.VisualLane == 0 ? next.Lane : next.VisualLane;
            var pos = Column.FullPositionAt(next.Time, l, next.ScrollGroup, next.StartEasing);
            var delta = pos - Position;
            var distance = delta.Length;

            followLine.Alpha = 1;
            followLine.Position = delta / 2;
            followLine.Height = distance;
            followLine.Rotation = -(float)(Math.Atan2(delta.X, delta.Y) * (180 / Math.PI));
        }
        else
            followLine.Alpha = 0;
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            var off = lagCompensation();
            ApplyResult(off ?? HitWindows.TimingFor(HitWindows.Lowest));
            return;
        }

        if (offset >= 0 && !directHit)
            return;

        if (wouldMiss)
        {
            var off = lagCompensation();

            if (off != null)
            {
                ApplyResult(off.Value);
                return;
            }
        }

        ObjectManager.PlayHitSound(Data, false);
        ApplyResult(lagCompensation() ?? offset);
        return;

        double? lagCompensation()
        {
            double off;

            if (isBeingHeld && holdStartTime != null)
            {
                var delta = holdStartTime.Value - Data.Time;

                off = delta < 0 ? 0 : delta;
            }
            else
                return null;

            return off;
        }
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        var flWindow = HitWindows.TimingFor(Judgement.Flawless);

        if (Math.Abs(TimeDelta) < flWindow)
        {
            directHit = true;
            UpdateJudgement(true);
            return;
        }

        isBeingHeld = true;

        var idx = input.Keys.IndexOf(key);
        holdStartTime = input.PressTimes[idx];
    }

    public override void OnReleased(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        // believe it or not
        // this works fine
        UpdateJudgement(true);

        isBeingHeld = false;
        holdStartTime = null;
    }
}
