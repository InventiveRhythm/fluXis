using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.UserInterface;
using fluXis.Input;
using fluXis.Online.Activity;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Multiplayer;

public partial class MultiSubScreen : Screen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public virtual string Title => "Screen";
    public virtual string SubTitle => "Screen";

    protected bool IsCurrentScreen => this.IsCurrentScreen() && MultiScreen.IsCurrentScreen();

    [Resolved]
    protected MultiplayerScreen MultiScreen { get; private set; }

    public Bindable<UserActivity> Activity => MultiScreen.Activity;
    protected virtual UserActivity InitialActivity => new UserActivity.MenuGeneral();
    public virtual bool AllowMusicPausing => false;

    private ScreenHeader header;

    protected override void LoadComplete()
    {
        AddInternal(header = new ScreenHeader());
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        RefreshTitle();
        this.FadeOut();

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
            FadeIn();
    }

    public override void OnSuspending([CanBeNull] ScreenTransitionEvent e) => FadeOut(e?.Next);

    public override void OnResuming(ScreenTransitionEvent e)
    {
        RefreshTitle();
        this.FadeOut();

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
            FadeIn();
    }

    public override bool OnExiting([CanBeNull] ScreenExitEvent e)
    {
        FadeOut(e?.Next);
        return false;
    }

    protected virtual void FadeIn()
    {
        RefreshActivity();

        header.Show();
        this.FadeInFromZero(Styling.TRANSITION_FADE);
    }

    protected virtual void FadeOut(IScreen next)
    {
        header.Hide();
        this.FadeOut(Styling.TRANSITION_FADE);
    }

    protected void RefreshTitle()
    {
        header.Title = Title;
        header.SubTitle = SubTitle;
    }

    protected void RefreshActivity() => Activity.Value = InitialActivity;

    protected void ApplyMapBackground(RealmMap map) => MultiScreen.ApplyMapBackground(map);

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
