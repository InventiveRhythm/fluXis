using fluXis.Game.Graphics.Shaders.Chromatic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Tests.Graphics;

public partial class TestChromaticShader : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new ChromaticContainer<Drawable>
        {
            RelativeSizeAxes = Axes.Both,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.5f),
                Colour = Color4.Red
            }
        });
    }
}
