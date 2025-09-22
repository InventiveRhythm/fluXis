using System;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Allocation;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableLandmine : DrawableHitObject
{
    public override bool CanBeRemoved => Judged || wouldMiss;

    private bool wouldMiss => Time.Current - Data.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    [Resolved]
    private GameplayInput input { get; set; }

    public DrawableLandmine(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new[]
        {
            Skin.GetLandmine(VisualLane, ObjectManager.KeyCount, Data.HoldTime > 0)
        };
    }

    protected override void Update()
    {
        base.Update();

        //judge the landmine as soon as it leaves the perfect timing window
        if (TimeDelta <= -HitWindows.TimingFor(Judgement.Perfect))
            UpdateJudgement(false);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(0);
            return;
        }

        if (Math.Abs(offset) <= HitWindows.TimingFor(Judgement.Perfect))
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        UpdateJudgement(true);
    }
}
