using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Online.Activity;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

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

    private Container titleContainer;
    private Circle titleLine;
    private FluXisSpriteText titleText;
    private FluXisSpriteText titleSubText;

    protected override void LoadComplete()
    {
        AddRangeInternal(new Drawable[]
        {
            titleContainer = new Container
            {
                AutoSizeAxes = Axes.Both,
                Margin = new MarginPadding(30),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Size = new Vector2(6, 3),
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Colour = FluXisColors.Text,
                        Child = titleLine = new Circle
                        {
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding { Left = 12 },
                        Children = new Drawable[]
                        {
                            titleText = new FluXisSpriteText
                            {
                                WebFontSize = 16,
                                Alpha = .8f
                            },
                            titleSubText = new FluXisSpriteText
                            {
                                WebFontSize = 20
                            }
                        }
                    }
                }
            }
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        RefreshTitle();
        this.FadeOut();

        using (BeginDelayedSequence(FluXisScreen.ENTER_DELAY))
            FadeIn();
    }

    public override void OnSuspending([CanBeNull] ScreenTransitionEvent e) => FadeOut(e?.Next);

    public override void OnResuming(ScreenTransitionEvent e)
    {
        RefreshTitle();
        this.FadeOut();

        using (BeginDelayedSequence(FluXisScreen.ENTER_DELAY))
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

        titleContainer.MoveToX(0);
        titleLine.ResizeHeightTo(0).ResizeHeightTo(1, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        titleText.MoveToX(-10).MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        titleSubText.MoveToX(-10).MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        this.FadeInFromZero(FluXisScreen.FADE_DURATION);
    }

    protected virtual void FadeOut(IScreen next)
    {
        titleContainer.MoveToX(50, FluXisScreen.MOVE_DURATION);
        this.FadeOut(FluXisScreen.FADE_DURATION);
    }

    protected void RefreshTitle()
    {
        titleText.Text = Title;
        titleSubText.Text = SubTitle;
    }

    protected void RefreshActivity() => Activity.Value = InitialActivity;

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
