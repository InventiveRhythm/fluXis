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

            if (subTitleText != null)
                subTitleText.Text = subTitle;
        }
    }

    private string subTitle;

    private TruncatingText subTitleText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Padding = new MarginPadding(10);

        InternalChildren = new Drawable[]
        {
            new TruncatingText
            {
                RelativeSizeAxes = Axes.X,
                Text = Title,
                WebFontSize = 16
            },
            subTitleText = new TruncatingText
            {
                RelativeSizeAxes = Axes.X,
                Text = SubTitle,
                WebFontSize = 12,
                Alpha = .8f
            }
        };
    }
}
