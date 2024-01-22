using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Select.Footer.Options;

public partial class FooterOptionSection : FillFlowContainer
{
    public string Title { get; init; }

    public string SubTitle
    {
        get => subTitle;
        set
        {
            subTitle = value;

            if (IsLoaded)
                subTitleText.Text = subTitle;
        }
    }

    private string subTitle;

    private FluXisSpriteText subTitleText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Padding = new MarginPadding(10);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                RelativeSizeAxes = Axes.X,
                Text = Title,
                WebFontSize = 16,
                Truncate = true
            },
            subTitleText = new FluXisSpriteText
            {
                RelativeSizeAxes = Axes.X,
                Text = SubTitle,
                WebFontSize = 12,
                Truncate = true,
                Alpha = .8f
            }
        };
    }
}
