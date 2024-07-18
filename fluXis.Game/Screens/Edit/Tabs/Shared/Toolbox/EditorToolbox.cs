using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;

public partial class EditorToolbox : ExpandingContainer
{
    private const int padding = 5;
    private const int size_closed = 48 + padding * 4;
    private const int size_open = 240 + padding * 4;

    protected override double HoverDelay => 500;

    public ToolboxCategory[] Categories { get; init; } = Array.Empty<ToolboxCategory>();

    private FillFlowContainer<ToolboxCategory> categories;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMediumNoOffset;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(padding),
                ScrollbarVisible = false,
                Child = categories = new FillFlowContainer<ToolboxCategory>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 10),
                    Children = Categories
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : size_closed, 500, Easing.OutQuart);
            categories.ForEach(d => d.OnSizeChanged(v.NewValue));
        }, true);
    }
}
