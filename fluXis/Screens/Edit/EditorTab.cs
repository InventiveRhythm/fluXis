using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Screens.Edit;

public abstract partial class EditorTab : Container
{
    public abstract IconUsage Icon { get; }
    public abstract string TabName { get; }
    public abstract EditorTabType Type { get; }

    public Action OnFullyLoaded;

    protected EditorTab()
    {
        Alpha = 0;
        RelativeSizeAxes = Axes.Both;
        RelativePositionAxes = Axes.Both;
    }

    protected void InvokeFullyLoaded() => OnFullyLoaded?.Invoke();
}

public enum EditorTabType {
    None,
    Wip,
    Setup,
    Charting,
    Design,
    Storyboard,
    Verify
}
