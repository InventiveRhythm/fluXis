using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.Background;
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
    protected virtual bool BackgroundAllowStoryboard => false;
    public virtual bool AllowMusicControl => true;
    public virtual bool AllowMusicPausing => AllowMusicControl;
    public virtual bool ShowCursor => true;
    public virtual bool ApplyValuesAfterLoad => false;
    public virtual bool AllowExit => true;
    public virtual bool PlayBackSound => true;
    public virtual bool AutoPlayNext => false;

    public virtual UserActivity InitialActivity => new UserActivity.MenuGeneral();
    public Bindable<UserActivity> Activity { get; }

    public BindableBool AllowOverlays { get; }

    [Resolved]
    protected GlobalBackground Backgrounds { get; private set; }

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

    public virtual void ApplyMapBackground(RealmMap map)
    {
        var draw = BackgroundAllowStoryboard ? new BlurableBackgroundWithStoryboard(map, BackgroundBlur) : new BlurableBackground(map, BackgroundBlur);
        Backgrounds.PushBackground(draw);
    }
}
