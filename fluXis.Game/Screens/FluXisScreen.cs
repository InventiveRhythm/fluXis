using fluXis.Game.Online.Activity;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
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
    public virtual bool ApplyValuesAfterLoad => false;
    public virtual bool AllowExit => true;
    public virtual bool PlayBackSound => true;

    public virtual UserActivity InitialActivity => new UserActivity.MenuGeneral();
    public Bindable<UserActivity> Activity { get; }

    private Sample backSample;

    protected new FluXisGameBase Game => base.Game as FluXisGameBase;

    protected FluXisScreen()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Activity = new Bindable<UserActivity>(InitialActivity);
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        backSample = samples.Get("UI/back");
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (PlayBackSound)
            backSample.Play();

        return base.OnExiting(e);
    }
}
