using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.Containers;

public partial class FullInputBlockingContainer : Container
{
    public Action OnClickAction { get; set; }

    protected override bool Handle(UIEvent e)
    {
        if (e is TouchEvent)
            return false;

        return true;
    }

    protected override bool OnClick(ClickEvent e)
    {
        OnClickAction?.Invoke();
        return true;
    }
}
