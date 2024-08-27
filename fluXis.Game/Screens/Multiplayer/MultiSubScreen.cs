using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online.Activity;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer;

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
    private Box titleLine;
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
                        Width = 5,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Scale = new Vector2(1, 3),
                        Child = titleLine = new Box
                        {
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                    titleText = new FluXisSpriteText
                    {
                        Text = Title,
                        Margin = new MarginPadding { Left = 10 },
                        FontSize = 40
                    },
                    titleSubText = new FluXisSpriteText
                    {
                        Text = SubTitle,
                        FontSize = 28,
                        Margin = new MarginPadding { Top = 30, Left = 10 },
                        Colour = FluXisColors.Text2
                    }
                }
            }
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        fadeIn();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        fadeOut();
        return false;
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        fadeOut();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        fadeIn();
    }

    private void fadeIn()
    {
        RefreshActivity();

        titleContainer.MoveToX(0);
        titleLine.ResizeHeightTo(0).ResizeHeightTo(1, 500, Easing.OutQuint);
        titleText.MoveToX(-10).MoveToX(0, 300, Easing.OutQuint);
        titleSubText.MoveToX(-10).MoveToX(0, 300, Easing.OutQuint);
        this.FadeInFromZero(200);
    }

    private void fadeOut()
    {
        titleContainer.MoveToX(50, 400);
        this.FadeOut(200);
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
