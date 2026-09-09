using System;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Modes.Keys.HitObjects;

public partial class DrawableLandmine : DrawableHitObject
{
    public override HitWindows HitWindows => Ruleset.LandmineWindows;

    public override bool CanBeRemoved => Judged || didNotGetHit;

    private bool didNotGetHit
    {
        get
        {
            // if the landmine is in the timing window of the next regular or long note, judge it as soon as it passes the receptors
            if (nextNote != null && TimeDelta <= 0)
                return true;

            return TimeDelta <= -HitWindows.TimingFor(Judgement.Miss);
        }
    }

    [Resolved]
    private GameplayInput input { get; set; }

    private bool isBeingHeld => Keybinds.Pressed[Array.IndexOf(Keybinds.Keys, Keybind)];

    // next non-landmine HitObject on the column. Only set if the landmine is in the hit window of the next note, null otherwise.
    private HitObject nextNote;

    public DrawableLandmine(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new[]
        {
            Skin.GetLandmine(VisualLane, ObjectManager.KeyCount).With(d => d.RelativeSizeAxes = Axes.X),
        };

        AlwaysPresent = true;
        if (Data.Hidden) Alpha = 0;

        nextNote = findNextNote();
    }

    protected override void Update()
    {
        base.Update();

        if (isBeingHeld)
            UpdateJudgement(true);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            if (isBeingHeld)
            {
                ApplyResult(0);
                return;
            }

            ApplyResult(-HitWindows.TimingFor(Judgement.Flawless));
            return;
        }

        if (isBeingHeld)
        {
            if (offset < 0)
                ApplyResult(0);
            return;
        }

        if (HitWindows.CanBeHit(offset))
            ApplyResult(offset);
    }

    protected override bool OnPressed(FluXisGameplayKeybind action)
    {
        if (action != Keybind)
            return false;

        if (!Column.IsFirst(this))
            return false;

        UpdateJudgement(true);
        return true;
    }

    private HitObject findNextNote()
    {
        HitObject next = Data.NextObject;

        while (next != null)
        {
            if (next.Time - Data.Time > Ruleset.HitWindows.TimingFor(Judgement.Okay)) return null; // no need to keep going if the next note is too far away
            if (next.Type != HitObjectType.Landmine) return next;

            next = next.NextObject;
        }

        return null;
    }
}
