using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

public partial class FluXisSearchBox : Container
{
    public FluXisTextBox TextBox { get; private set; }
    public Action OnTextChanged { get; set; }

    private FluXisSpriteIcon searchIcon;
    private LoadingIcon loadingIcon;

    public string StatusText { get; set; } = "";

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            searchIcon = new FluXisSpriteIcon
            {
                Size = new Vector2(20),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Icon = FontAwesome6.Solid.MagnifyingGlass,
                Margin = new MarginPadding { Left = 15 },
                Alpha = 1f
            },
            loadingIcon = new LoadingIcon()
            {
                Size = new Vector2(25),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 15 },
                Alpha = 0f
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 40, Right = 10 },
                Children = new Drawable[] {
                    TextBox = new FluXisTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 40,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        PlaceholderText = "Click to search...",
                        BackgroundActive = Theme.Background2,
                        BackgroundInactive = Theme.Background2,
                        OnTextChanged = OnTextChanged
                    }
                }

            },
        };
    }

    public void ShowLoading()
    {
        searchIcon.FadeOut(200);
        loadingIcon.FadeIn(200);
    }

    public void HideLoading()
    {
        loadingIcon.FadeOut(200);
        searchIcon.FadeIn(200);
    }
}