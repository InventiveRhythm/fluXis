using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.Containers;

public partial class FullInputBlockingContainer : Container
{
    public List<Type> Allow { get; set; } = new();

    public Action OnClickAction { get; set; }

    protected override bool Handle(UIEvent e)
    {
        if (e is TouchEvent)
            return false;

        return !Allow.Contains(e.GetType());
    }

    protected override bool OnClick(ClickEvent e)
    {
        OnClickAction?.Invoke();
        return true;
    }
}
