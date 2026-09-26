using fluXis.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Mode.Keys.Objects.Drawable;

public partial class DrawableNote : DrawableKeysHitObject<Note>, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public override bool CanBeRemoved => Judged || Time.Current - Object.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    public DrawableNote(Note o)
        : base(o)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetHitObject(PlayerLane, Manager.KeyCount).With(d => d.RelativeSizeAxes = Axes.X);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(HitWindows.Lowest);
            return;
        }

        if (!HitWindows.CanBeHit(offset))
            return;

        var result = HitWindows.JudgementFor(offset);
        ApplyResult(result);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e)
        => e.Action == Action && UpdateJudgement(true);

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }
}
