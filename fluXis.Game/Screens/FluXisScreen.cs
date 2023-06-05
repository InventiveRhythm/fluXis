using osu.Framework.Screens;

namespace fluXis.Game.Screens;

public partial class FluXisScreen : Screen
{
    public virtual float Zoom => 1f;
    public virtual float ParallaxStrength => 10f;
    public virtual bool ShowToolbar => true;
    public virtual float BackgroundDim => 0.25f;
    public virtual float BackgroundBlur => 0f;
    public virtual bool AllowMusicControl => true;
}
