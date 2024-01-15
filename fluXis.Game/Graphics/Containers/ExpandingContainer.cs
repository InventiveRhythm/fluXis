using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Threading;

namespace fluXis.Game.Graphics.Containers;

public partial class ExpandingContainer : Container
{
    public BindableBool Expanded { get; } = new();
    public BindableBool Locked { get; } = new();

    protected virtual double HoverDelay => 0;
    private ScheduledDelegate hoverDelayEvent;

    protected ExpandingContainer()
    {
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (Locked.Value) return true;

        hoverDelayEvent = Scheduler.AddDelayed(() => Expanded.Value = true, HoverDelay);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (Locked.Value) return;

        hoverDelayEvent?.Cancel();
        if (Expanded.Value) Expanded.Value = false;
    }
}
