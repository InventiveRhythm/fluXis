using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;
using fluXis.Game.Screens.Multiplayer.SubScreens.Ranked;
using fluXis.Game.UI;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens;

public partial class MultiModeSelect : MultiSubScreen
{
    public override string Title => "Multiplayer";
    public override string SubTitle => "Mode Select";

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    private readonly Bindable<Mode> mode = new();

    private Box redBox;
    private Container buttons;
    private MultiModeButton rankedButton;
    private MultiModeButton openLobbyButton;
    private CornerButton backButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            redBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(1.1f),
                Alpha = 0,
                Colour = FluXisColors.Red
            },
            buttons = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    rankedButton = new MultiModeButton
                    {
                        /*Title = "Ranked",
                        Desciption = "Compete with other players in 1v1\nmatches to climb the leaderboard.",*/
                        Title = "???",
                        Desciption = "???",
                        Background = "ranked",
                        RightSide = false,
                        Locked = true,
                        // Action = () => this.Push(new MultiRankedMain()),
                        Action = () =>
                        {
                            this.Shake(200, 15);
                            redBox.FadeTo(.5f).FadeOut(600);
                        },
                        HoverAction = () => mode.Value = Mode.Ranked,
                        HoverLostAction = () => mode.Value = Mode.None
                    },
                    openLobbyButton = new MultiModeButton
                    {
                        Title = "Open Lobby",
                        Desciption = "Play freely against your\nfriends with no restrictions.",
                        Background = "lobby",
                        RightSide = true,
                        Action = () => this.Push(new MultiLobbyList()),
                        HoverAction = () => mode.Value = Mode.OpenLobby,
                        HoverLostAction = () => mode.Value = Mode.None
                    }
                }
            },
            backButton = new CornerButton
            {
                Corner = Corner.BottomLeft,
                ButtonText = "Back",
                Icon = FontAwesome.Solid.ChevronLeft,
                Action = this.Exit
            },
            new FluXisSpriteText
            {
                Text = "Multiplayer is very much a work in progress. Nothing here really works.",
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Shadow = true,
                FontSize = 40,
                Y = -100
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        mode.BindValueChanged(e =>
        {
            if (e.NewValue == Mode.Ranked)
                rankedButton.Select();
            else
                rankedButton.Deselect();

            if (e.NewValue == Mode.OpenLobby)
                openLobbyButton.Select();
            else
                openLobbyButton.Deselect();
        }, true);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        menuMusic.GoToLayer(0, -1);

        rankedButton.Show();
        openLobbyButton.Show();
        backButton.Show();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        rankedButton.Hide();
        openLobbyButton.Hide();
        backButton.Hide();
        return base.OnExiting(e);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);

        var up = e.Next is MultiRankedMain;
        buttons.MoveToY(up ? -100 : 100, 400, Easing.OutQuint);

        rankedButton.Hide();
        openLobbyButton.Hide();
        backButton.Hide();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);

        menuMusic.GoToLayer(0, -1);
        buttons.MoveToY(0, 400, Easing.OutQuint);

        rankedButton.Show();
        openLobbyButton.Show();
        backButton.Show();
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.PreviousGroup:
                mode.Value = Mode.Ranked;
                return true;

            case FluXisGlobalKeybind.NextGroup:
                mode.Value = Mode.OpenLobby;
                return true;

            case FluXisGlobalKeybind.Select:
                switch (mode.Value)
                {
                    case Mode.Ranked:
                        rankedButton.Trigger();
                        return true;

                    case Mode.OpenLobby:
                        openLobbyButton.Trigger();
                        return true;
                }

                return true;
        }

        return base.OnPressed(e);
    }

    private enum Mode
    {
        None,
        Ranked,
        OpenLobby
    }
}
