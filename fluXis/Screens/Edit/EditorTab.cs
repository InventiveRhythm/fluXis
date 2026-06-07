using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Screens.Edit;

public abstract partial class EditorTab : Container
{
    public abstract IconUsage Icon { get; }
    public abstract string TabName { get; }
    public virtual bool HasLoading => false;

    protected EditorTab()
    {
        Alpha = 0;
        RelativeSizeAxes = Axes.Both;
        RelativePositionAxes = Axes.Both;
    }

    public virtual void ScheduleAfterLoad(Action act) { }
}
