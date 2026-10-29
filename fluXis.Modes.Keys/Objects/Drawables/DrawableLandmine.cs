using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Modes.Keys.Objects.Drawables;

public partial class DrawableLandmine : DrawableKeysHitObject<Landmine>, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public override bool CanBeRemoved => Judged || missed;

    private bool heldDown;
    private HitObject? nextObject;

    private bool missed
    {
        get
        {
            // if the landmine is in the timing window of the next regular or long note, judge it as soon as it passes the receptors
            if (nextObject != null && TimeDelta <= 0)
                return true;

            return TimeDelta <= -HitWindows.TimingFor(Judgement.Miss);
        }
    }

    public DrawableLandmine(Landmine o)
        : base(o)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetLandmine(PlayerLane, Manager.KeyCount).With(d => d.RelativeSizeAxes = Axes.X);

        AlwaysPresent = true;
        if (Object.Hidden) Alpha = 0;

        nextObject = findNextNote();
    }

    protected override void Update()
    {
        base.Update();

        heldDown = Keybinds.PressedActions.Contains(Action);
        if (heldDown) UpdateJudgement(true);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(Judgement.Flawless, Object.Time);
            return;
        }

        if (heldDown)
        {
            if (offset < 0)
                ApplyResult(Judgement.Miss);

            return;
        }

        if (HitWindows.CanBeHit(offset))
            ApplyResult(HitWindows.JudgementFor(offset));
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e)
        => e.Action == Action && UpdateJudgement(true);

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }

    private HitObject? findNextNote()
    {
        HitObject? next = Object.NextObject;

        while (next != null)
        {
            if (next.Time - Object.Time > Ruleset.HitWindows.TimingFor(Judgement.Okay)) return null; // no need to keep going if the next note is too far away
            if (next is not Landmine) return next;

            next = next.NextObject;
        }

        return null;
    }
}
