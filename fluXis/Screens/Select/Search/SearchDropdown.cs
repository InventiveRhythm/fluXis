using fluXis.Graphics;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Select.Search.Dropdown;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Search;

public partial class SearchDropdown : Container
{
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
        EdgeEffect = Styling.ShadowMedium;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background1
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
                    new SearchDropdownKeymode(),
                    new SearchDropdownBPM(),
                    new SearchDropdownStatus()
                }
            }
        };
    }

    public override void Show() => this.FadeIn(200).MoveToY(-10, 400, Easing.OutQuint);
    public override void Hide() => this.FadeOut(200).MoveToY(-30, 400, Easing.OutQuint);

    protected override bool OnClick(ClickEvent e) => true;
}
