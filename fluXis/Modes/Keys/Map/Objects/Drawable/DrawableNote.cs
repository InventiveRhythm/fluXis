using fluXis.Input;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Modes.Keys.Map.Objects.Drawable;

#nullable enable

public partial class DrawableNote : DrawableKeysHitObject<HitObject>, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public override bool CanBeRemoved => Judged || Time.Current - Object.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    public DrawableNote(HitObject o)
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
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
            return;
        }

        if (!HitWindows.CanBeHit(offset))
            return;

        ApplyResult(offset);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e)
        => e.Action == Action && UpdateJudgement(true);

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }
}
