using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Shaders.Bloom;
using fluXis.Game.Graphics.Shaders.Chromatic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Tests.Graphics;

public partial class TestShaderContainer : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddStep("Clear", Clear);
        AddStep("Add Chromatic", () => createContainer(new ChromaticContainer()));
        AddStep("Add Bloom", () => createContainer(new BufferedBloomContainer()));
    }

    private void createContainer(Container<Drawable> shader)
    {
        Add(shader.With(s =>
        {
            s.RelativeSizeAxes = Axes.Both;
            s.Children = new Drawable[]
            {
                new ParallaxContainer
                {
                    Strength = .04f,
                    Child = new MapBackground
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FillMode = FillMode.Fill
                    }
                },
                new ParallaxContainer
                {
                    Strength = .06f,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(0.5f)
                    }
                }
            };
        }));
    }
}
