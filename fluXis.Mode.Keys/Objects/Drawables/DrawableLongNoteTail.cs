using fluXis.Input;
using fluXis.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Mode.Keys.Objects.Drawables;

public partial class DrawableLongNoteTail : DrawableKeysHitObject<LongNoteTail>, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public override bool CanBeRemoved => Judged || TimeDelta < -HitWindows.TimingFor(HitWindows.LowestHitable);

    private readonly DrawableLongNote parent;
    private bool valid => parent.Judged && parent.Tracked.Judgement != Judgement.Miss;

    public DrawableLongNoteTail(LongNoteTail o, DrawableLongNote parent)
        : base(o)
    {
        this.parent = parent;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetLongNoteEnd(PlayerLane, Manager.KeyCount).With(d => d.RelativeSizeAxes = Axes.X);
    }

    // handled by the parent
    protected override void UpdatePosition() { }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(HitWindows.Lowest);
            return;
        }

        if (!HitWindows.CanBeHit(offset))
        {
            ApplyResult(HitWindows.Lowest);
            return;
        }

        var result = HitWindows.JudgementFor(offset);
        ApplyResult(result);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => false;

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e)
    {
        if (!valid || Action != e.Action)
            return;

        UpdateJudgement(true);
    }
}
