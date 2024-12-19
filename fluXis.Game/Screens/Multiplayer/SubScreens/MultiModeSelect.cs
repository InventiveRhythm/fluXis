using fluXis.Game.Graphics.Shaders.Bloom;
using fluXis.Game.Graphics.Shaders.Chromatic;
using fluXis.Game.Graphics.Shaders.Glitch;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Online.Activity;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;
using fluXis.Game.Screens.Multiplayer.SubScreens.Ranked;
using fluXis.Game.UI;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens;

public partial class MultiModeSelect : MultiSubScreen
{
    public override string Title => "Multiplayer";
    public override string SubTitle => "Mode Select";

    protected override UserActivity InitialActivity => new UserActivity.MenuGeneral();

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    private readonly Bindable<Mode> mode = new();

    private const int max_clicks = 10;
    private int clickCount;
    private FluXisTextFlow hiddenText;

    private GlitchContainer glitch;
    private ChromaticContainer chroma;

    private Box redBox;
    private Container buttons;
    private MultiModeButton rankedButton;
    private MultiModeButton openLobbyButton;
    private CornerButton backButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = glitch = new GlitchContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = chroma = new ChromaticContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    redBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(2f),
                        Alpha = 0,
                        Colour = FluXisColors.Red
                    },
                    buttons = new BloomContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            hiddenText = new FluXisTextFlow
                            {
                                AutoSizeAxes = Axes.X,
                                TextAnchor = Anchor.TopCentre,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                X = 200,
                                FontSize = 48,
                                Alpha = 0
                            },
                            rankedButton = new MultiModeButton
                            {
                                /*Title = "Ranked",
                                Description = "Compete with other players in 1v1\nmatches to climb the leaderboard.",*/
                                Title = "???",
                                Description = "???",
                                Background = "ranked",
                                RightSide = false,
                                Locked = true,
                                // Action = () => this.Push(new MultiRankedMain()),
                                Action = () =>
                                {
                                    this.Shake(200, 15);
                                    redBox.FadeTo(.5f).FadeOut(600);

                                    chroma.StrengthTo(16).StrengthTo(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
                                    glitch.StrengthTo(1).Strength2To(1).Strength3To(.04f)
                                          .StrengthTo(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
                                          .Strength2To(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
                                          .Strength3To(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

                                    if (++clickCount < max_clicks)
                                        return;

                                    rankedButton.FadeOut();
                                    hiddenText.FadeIn();
                                },
                                HoverAction = () => mode.Value = Mode.Ranked,
                                HoverLostAction = () => mode.Value = Mode.None
                            },
                            openLobbyButton = new MultiModeButton
                            {
                                Title = "Open Lobby",
                                Description = "Play freely against your\nfriends with no restrictions.",
                                Background = "lobby",
                                RightSide = true,
                                Action = () => OpenList(),
                                HoverAction = () => mode.Value = Mode.OpenLobby,
                                HoverLostAction = () => mode.Value = Mode.None
                            }
                        }
                    },
                    backButton = new CornerButton
                    {
                        Corner = Corner.BottomLeft,
                        ButtonText = LocalizationStrings.General.Back,
                        Icon = FontAwesome6.Solid.AngleLeft,
                        Action = this.Exit
                    },
                    new FluXisSpriteText
                    {
                        Text = "Multiplayer is very much a work in progress. Expect issues.",
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Shadow = true,
                        WebFontSize = 24,
                        Y = -100
                    }
                }
            },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        hiddenText.AddParagraph("too much clicking");
        hiddenText.AddParagraph("no more button for you", t =>
        {
            var text = (FluXisSpriteText)t;
            text.FontSize = 24;
            text.Alpha = .8f;
        });

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

    public void OpenList(long id = -1, string password = "")
        => Schedule(() => this.Push(new MultiLobbyList(id, password)));

    protected override void FadeIn()
    {
        base.FadeIn();

        buttons.MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

        menuMusic.GoToLayer(0, -1);

        rankedButton.Show();
        openLobbyButton.Show();
        backButton.Show();
    }

    protected override void FadeOut(IScreen next)
    {
        base.FadeOut(next);

        var up = next is MultiRankedMain;
        buttons.MoveToY(up ? -100 : 100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

        rankedButton.Hide();
        openLobbyButton.Hide();
        backButton.Hide();
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
