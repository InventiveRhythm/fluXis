using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay;

public partial class FullComboOverlay : Container
{
    private readonly StripePattern pattern;
    private readonly SpriteText text;

    public FullComboOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientHorizontal(FluXisColors.Accent, FluXisColors.Accent4),
                Alpha = 0.2f
            },
            pattern = new StripePattern
            {
                Speed = new Vector2(-300)
            },
            text = new SpriteText
            {
                Font = new FontUsage("Quicksand", 100, "Bold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    public void Show(FullComboType type)
    {
        this.FadeIn(200);
        pattern.SpeedTo(new Vector2(-50), 500, Easing.OutQuint);
        text.ScaleTo(1.1f, 3000, Easing.OutQuint);

        text.Text = type switch
        {
            FullComboType.FullCombo => "FULL COMBO",
            FullComboType.AllFlawless => "ALL FLAWLESS",
            _ => text.Text
        };
    }

    public enum FullComboType
    {
        FullCombo,
        AllFlawless
    }
}
