using System;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Allocation;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableLandmine : DrawableHitObject
{
    public override bool CanBeRemoved => Judged || didNotGetHit;

    private bool didNotGetHit => TimeDelta <= -HitWindows.TimingFor(Judgement.Perfect);

    [Resolved]
    private GameplayInput input { get; set; }

    private bool isBeingHeld;

    public DrawableLandmine(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        //Masking = false; //needed for using the cross in bar skin
        InternalChildren = new[]
        {
            Skin.GetLandmine(VisualLane, ObjectManager.KeyCount)
        };
    }

    protected override void Update()
    {
        base.Update();

        //if the landmine is in the timing window of the next regular or long note, judge it as soon as it passes the receptors
        HitObject next = Data.NextObject;

        if (next != null && next.Type != 2 && next.Time - Data.Time < HitWindows.TimingFor(Judgement.Okay) && TimeDelta <= 0)
        {
            UpdateJudgement(false);
        }

        //TODO (?): make it more forgiving if the landmine is too close of the previous note?
        if (isBeingHeld)
            UpdateJudgement(true);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(0);
            return;
        }

        if (isBeingHeld)
        {
            if (offset < 0)
                ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
            return;
        }

        if (Math.Abs(offset) <= HitWindows.TimingFor(Judgement.Perfect))
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        if (Column.IsFirst(this))
            UpdateJudgement(true);

        isBeingHeld = true;
    }

    public override void OnReleased(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        isBeingHeld = false;
    }
}
