using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Tests.Graphics;

public partial class TestLoadingIcon : FluXisTestScene
{
    public TestLoadingIcon()
    {
        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(100)
            }
        });
    }
}
