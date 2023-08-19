using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens.Select.Search.Dropdown;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchDropdown : Container
{
    public SearchFilters Filters { get; init; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.BottomLeft;
        CornerRadius = 10;
        Masking = true;
        Y = -30;
        Alpha = 0;
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Color4.Black.Opacity(0.25f),
            Radius = 10,
            Offset = new Vector2(0, 1)
        };

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background1
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                Padding = new MarginPadding(10) { Left = 30, Top = 20 },
                Children = new Drawable[]
                {
                    new SearchDropdownBPM { Filters = Filters },
                    new SearchDropdownStatus { Filters = Filters },
                }
            }
        };
    }

    public override void Show() => this.FadeIn(200).MoveToY(-10, 400, Easing.OutQuint);
    public override void Hide() => this.FadeOut(200).MoveToY(-30, 400, Easing.OutQuint);
}
