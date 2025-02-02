using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Layout.Components;

public partial class LayoutListComponent : CompositeDrawable
{
    [Resolved]
    private LayoutEditor editor { get; set; }

    [Resolved]
    private LayoutManager layouts { get; set; }

    private string key { get; }
    private Type type { get; }

    public LayoutListComponent(string key, Type type)
    {
        this.key = key;
        this.type = type;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        var settings = new HUDComponentSettings();
        var comp = layouts.CreateComponent(key, settings, editor.JudgementProcessor, editor.HealthProcessor, editor.ScoreProcessor, editor.HitWindows);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(12),
                Child = comp
            }
        };
    }
}
