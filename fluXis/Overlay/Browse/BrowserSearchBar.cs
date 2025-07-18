using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Browse;

public partial class BrowserSearchBar : Container
{
    public Action<string> OnSearch { get; set; }

    private FluXisTextBox textBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;
        CornerRadius = 10;
        Masking = true;
        // EdgeEffect = FluXisStyles.ShadowSmall;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 60 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 10,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Background3
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(10),
                            Child = textBox = new FluXisTextBox
                            {
                                RelativeSizeAxes = Axes.Both,
                                PlaceholderText = "Search...",
                                FontSize = FluXisSpriteText.GetWebFontSize(20),
                                BackgroundInactive = Theme.Background3,
                                BackgroundActive = Theme.Background3
                            }
                        }
                    }
                }
            },
            new FluXisSpriteIcon
            {
                Size = new Vector2(25),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                X = 30,
                Icon = FontAwesome6.Solid.MagnifyingGlass
            },
            new ClickableSpriteIcon
            {
                Size = new Vector2(25),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.Centre,
                X = -30,
                Icon = FontAwesome6.Solid.AngleDoubleRight,
                Action = search
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        textBox.OnCommit += (_, _) => search();
    }

    private void search()
    {
        OnSearch?.Invoke(textBox.Text);
    }
}
