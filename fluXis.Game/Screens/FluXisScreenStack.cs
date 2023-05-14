using fluXis.Game.Graphics.Background;
using fluXis.Game.Overlay.Toolbar;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens;

public partial class FluXisScreenStack : ScreenStack
{
    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private Toolbar toolbar { get; set; }

    public FluXisScreenStack()
    {
        RelativeSizeAxes = Axes.Both;
        ScreenPushed += updateValues;
        ScreenExited += updateValues;
    }

    private void updateValues(IScreen last, IScreen next)
    {
        if (next is FluXisScreen fluXisScreen)
        {
            backgrounds.Zoom = fluXisScreen.Zoom;
            backgrounds.ParallaxStrength = fluXisScreen.ParallaxStrength;
            toolbar.ShowToolbar.Value = fluXisScreen.ShowToolbar;
            backgrounds.SetBlur(fluXisScreen.BackgroundBlur);
            backgrounds.SetDim(fluXisScreen.BackgroundDim);
        }
        else
        {
            backgrounds.Zoom = 1f;
            backgrounds.ParallaxStrength = 10f;
            backgrounds.SetBlur(0f);
            backgrounds.SetDim(0.25f);
        }
    }
}
