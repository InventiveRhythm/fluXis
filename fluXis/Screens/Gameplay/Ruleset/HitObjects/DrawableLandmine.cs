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

public partial class DrawableLandmine : DrawableHitObject
{
    public override bool CanBeRemoved => Judged || wouldMiss;

    private bool wouldMiss => Time.Current - Data.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    [Resolved]
    private GameplayInput input { get; set; }

    private bool isBeingHeld;
    private double? holdStartTime;

    private Circle followLine;

    public DrawableLandmine(HitObject data)
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
                Colour = Colour4.FromHex("#FF5252").Opacity(.4f),
                Size = new Vector2(8),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            Skin.GetLandmine(VisualLane, ObjectManager.KeyCount, Data.HoldTime > 0)
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

        if (next?.Type == 2 && next.Lane == Data.Lane && (Data.VisualLane != 0 || next.VisualLane != 0))
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
            ApplyResult(0);
            return;
        }

        //if (offset >= HitWindows.TimingFor(Judgement.Flawless))
        if (Math.Abs(offset) >= HitWindows.TimingFor(Judgement.Flawless) / 2)
            return;

        ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        isBeingHeld = true;

        var idx = input.Keys.IndexOf(key);
        holdStartTime = input.PressTimes[idx];
    }

    public override void OnReleased(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        isBeingHeld = false;
        holdStartTime = null;
    }
}
