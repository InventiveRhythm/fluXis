using fluXis.Game.Graphics.Background;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens;

public partial class FluXisScreenStack : ScreenStack
{
    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    public FluXisScreenStack()
    {
        RelativeSizeAxes = Axes.Both;
        ScreenPushed += screenPushed;
        ScreenExited += screenChanged;
    }

    private void screenChanged(IScreen last, IScreen next)
    {
        if (next is FluXisScreen fluXisScreen)
        {
            backgrounds.Zoom = fluXisScreen.Zoom;
            backgrounds.ParallaxStrength = fluXisScreen.ParallaxStrength;
        }
        else
        {
            backgrounds.Zoom = 1f;
            backgrounds.ParallaxStrength = 10f;
        }
    }

    private void screenPushed(IScreen last, IScreen next)
    {
        if (next is FluXisScreen fluXisScreen)
        {
            backgrounds.Zoom = fluXisScreen.Zoom;
            backgrounds.ParallaxStrength = fluXisScreen.ParallaxStrength;
        }
        else
        {
            backgrounds.Zoom = 1f;
            backgrounds.ParallaxStrength = 10f;
        }
    }
}
