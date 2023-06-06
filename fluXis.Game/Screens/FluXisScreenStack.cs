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

    public bool AllowMusicControl => CurrentScreen is FluXisScreen { AllowMusicControl: true };

    public FluXisScreenStack()
    {
        RelativeSizeAxes = Axes.Both;
        ScreenPushed += updateScreen;
        ScreenExited += updateScreen;
    }

    private void updateScreen(IScreen last, IScreen next)
    {
        if (next is FluXisScreen fluXisScreen)
        {
            if (fluXisScreen.ApplyValuesAfterLoad && !fluXisScreen.IsLoaded) fluXisScreen.OnLoadComplete += _ => updateValues(fluXisScreen);
            else updateValues(fluXisScreen);
        }
        else
        {
            backgrounds.Zoom = 1f;
            backgrounds.ParallaxStrength = 10f;
            backgrounds.SetBlur(0f);
            backgrounds.SetDim(0.25f);
        }
    }

    private void updateValues(FluXisScreen screen)
    {
        backgrounds.Zoom = screen.Zoom;
        backgrounds.ParallaxStrength = screen.ParallaxStrength;
        toolbar.ShowToolbar.Value = screen.ShowToolbar;
        backgrounds.SetBlur(screen.BackgroundBlur);
        backgrounds.SetDim(screen.BackgroundDim);
    }
}
