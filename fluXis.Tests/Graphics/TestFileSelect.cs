using fluXis.Graphics.UserInterface.Files;
using fluXis.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Tests.Graphics;

public partial class TestFileSelect : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyBeatSync();

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
