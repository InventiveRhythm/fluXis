using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Tests.Gameplay;

public partial class TestStageBackground : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background4
            },
            new Container
            {
                Width = 500,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(.75f),
                Child = new Stage()
            }
        });
    }
}
