using osu.Framework.Screens;

namespace fluXis.Game.Screens;

public partial class FluXisScreen : Screen
{
    public virtual float Zoom => 1f;
    public virtual float ParallaxStrength => 10f;
    public virtual bool ShowToolbar => true;
}
