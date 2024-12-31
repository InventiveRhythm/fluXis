using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Select.UI;

public partial class SelectNoMaps : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        CornerRadius = 20;
        Masking = true;
        Alpha = 0;
        Scale = new Vector2(0.9f);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(20),
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = FontAwesome6.Solid.TriangleExclamation,
                        Size = new Vector2(30),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisSpriteText
                    {
                        Text = LocalizationStrings.SongSelect.NoMapsFound,
                        FontSize = 32,
                        Shadow = true,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisSpriteText
                    {
                        Text = LocalizationStrings.SongSelect.NoMapsFoundDescription,
                        FontSize = 26,
                        Colour = FluXisColors.Text2,
                        Shadow = true,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    }
                }
            }
        };
    }

    public override void Show() => this.FadeIn(400, Easing.OutQuint).ScaleTo(1, 1000, Easing.OutElastic);
    public override void Hide() => this.FadeOut(400, Easing.OutQuint).ScaleTo(0.9f, 400, Easing.OutQuint);
}
