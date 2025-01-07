using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Utils;
using osuTK.Graphics;

namespace fluXis.Tests.Graphics;

public partial class TestGradientText : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var normalText = new FluXisSpriteText
        {
            Text = "framework text",
            Anchor = Anchor.Centre,
            Origin = Anchor.BottomCentre,
            WebFontSize = 48
        };

        var gradientText = new GradientText
        {
            Text = "better gradient text",
            Anchor = Anchor.Centre,
            Origin = Anchor.TopCentre,
            WebFontSize = 48
        };

        Add(normalText);
        Add(gradientText);

        AddStep("Randomize colors", () =>
        {
            var col = getRandom();
            gradientText.Colour = col;
            normalText.Colour = col;
        });
    }

    private ColourInfo getRandom()
    {
        return new ColourInfo
        {
            TopLeft = new Color4(RNG.NextSingle(1), RNG.NextSingle(1), RNG.NextSingle(1), 1),
            TopRight = new Color4(RNG.NextSingle(1), RNG.NextSingle(1), RNG.NextSingle(1), 1),
            BottomLeft = new Color4(RNG.NextSingle(1), RNG.NextSingle(1), RNG.NextSingle(1), 1),
            BottomRight = new Color4(RNG.NextSingle(1), RNG.NextSingle(1), RNG.NextSingle(1), 1)
        };
    }
}
