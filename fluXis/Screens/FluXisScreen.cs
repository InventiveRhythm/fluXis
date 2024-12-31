using fluXis.Audio;
using fluXis.Online.Activity;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Screens;

public partial class FluXisScreen : Screen
{
    public virtual float Zoom => 1f;
    public virtual float ParallaxStrength => .05f;
    public virtual bool ShowToolbar => true;
    public virtual float BackgroundDim => 0.25f;
    public virtual float BackgroundBlur => 0f;
    public virtual bool AllowMusicControl => true;
    public virtual bool AllowMusicPausing => AllowMusicControl;
    public virtual bool ShowCursor => true;
    public virtual bool ApplyValuesAfterLoad => false;
    public virtual bool AllowExit => true;
    public virtual bool PlayBackSound => true;
    public virtual bool AutoPlayNext => false;

    // transition defaults
    public const float MOVE_DURATION = 600;
    public const float FADE_DURATION = 300;
    public const float ENTER_DELAY = 100;

    public virtual UserActivity InitialActivity => new UserActivity.MenuGeneral();
    public Bindable<UserActivity> Activity { get; }

    public BindableBool AllowOverlays { get; }

    [Resolved]
    protected UISamples UISamples { get; private set; }

    protected new FluXisGame Game => base.Game as FluXisGame;

    protected FluXisScreen()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Activity = new Bindable<UserActivity>(InitialActivity);
        AllowOverlays = new BindableBool(true);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (PlayBackSound)
            UISamples?.Back();

        return base.OnExiting(e);
    }
}
