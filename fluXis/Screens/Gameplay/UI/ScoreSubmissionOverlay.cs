using fluXis.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.UI;

public partial class ScoreSubmissionOverlay : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(300, 100);
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Margin = new MarginPadding { Bottom = 50 };
        CornerRadius = 20;
        Masking = true;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = FontAwesome6.Solid.Shapes,
                        Size = new Vector2(30),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Margin = new MarginPadding { Bottom = 10 }
                    },
                    new FluXisSpriteText
                    {
                        WebFontSize = 16,
                        Text = "Submitting Score...",
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Margin = new MarginPadding { Bottom = -4 }
                    },
                    new FluXisSpriteText
                    {
                        WebFontSize = 14,
                        Alpha = .8f,
                        Text = "Please wait...",
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    }
                }
            }
        };
    }
}
