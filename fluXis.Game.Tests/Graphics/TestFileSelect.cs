using fluXis.Game.Graphics.UserInterface.Files;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Game.Tests.Graphics;

public partial class TestFileSelect : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new MapBackground(null)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new FileSelect
            {
                MapDirectory = "C:/Users/Flux/Desktop",
                Size = new Vector2(1400, 700)
            }
        });
    }
}
