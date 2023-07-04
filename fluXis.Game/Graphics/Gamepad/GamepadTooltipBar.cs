using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.Gamepad;

public partial class GamepadTooltipBar : Container
{
    public GamepadTooltip[] TooltipsLeft { get; init; } = Array.Empty<GamepadTooltip>();
    public GamepadTooltip[] TooltipsRight { get; init; } = Array.Empty<GamepadTooltip>();
    public bool ShowBackground { get; init; } = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Anchor = Origin = Anchor.BottomCentre;

        if (ShowBackground)
        {
            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .5f
            });
        }

        Add(new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding { Horizontal = 20 },
            Children = new[]
            {
                new FillFlowContainer
                {
                    Direction = FillDirection.Horizontal,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Spacing = new Vector2(20),
                    Children = TooltipsLeft
                },
                new FillFlowContainer
                {
                    Direction = FillDirection.Horizontal,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Spacing = new Vector2(20),
                    Children = TooltipsRight
                }
            }
        });
    }
}
