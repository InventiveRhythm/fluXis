using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.Containers;

public partial class HoverClickContainer : ClickableContainer
{
    public Action HoverAction { get; set; }
    public Action HoverLostAction { get; set; }

    protected override bool OnHover(HoverEvent e)
    {
        HoverAction?.Invoke();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        HoverLostAction?.Invoke();
        base.OnHoverLost(e);
    }
}
