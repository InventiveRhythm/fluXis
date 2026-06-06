using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface;

public partial class ScreenHeader : CompositeDrawable
{
    public LocalisableString Title
    {
        get => title;
        set
        {
            title = value;

            if (titleSprite != null)
                titleSprite.Text = value;
        }
    }

    public LocalisableString SubTitle
    {
        get => subTitle;
        set
        {
            subTitle = value;

            if (subTitleSprite != null)
                subTitleSprite.Text = value;
        }
    }

    private LocalisableString title;
    private LocalisableString subTitle;

    private Drawable line;
    private FluXisSpriteText titleSprite;
    private FluXisSpriteText subTitleSprite;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Margin = new MarginPadding(32);

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(6, 3),
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Colour = Theme.Text,
                Child = line = new Circle
                {
                    RelativeSizeAxes = Axes.Both
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Margin = new MarginPadding { Left = 12 },
                Children = new Drawable[]
                {
                    titleSprite = new FluXisSpriteText
                    {
                        Text = title,
                        WebFontSize = 16,
                        Alpha = .8f
                    },
                    subTitleSprite = new FluXisSpriteText
                    {
                        Text = subTitle,
                        WebFontSize = 20
                    }
                }
            }
        };
    }

    public override void Show()
    {
        this.MoveToX(0);
        line.ResizeHeightTo(0).ResizeHeightTo(1, Styling.TRANSITION_MOVE, Easing.OutQuint);
        titleSprite.MoveToX(-10).MoveToX(0, Styling.TRANSITION_MOVE, Easing.OutQuint);
        subTitleSprite.MoveToX(-10).MoveToX(0, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.MoveToX(50, Styling.TRANSITION_MOVE);
    }
}
