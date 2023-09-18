using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultStageBackground : Box
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Colour = Colour4.Black;
        Alpha = 0.8f;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
    }
}
