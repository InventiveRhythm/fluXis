using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Panel;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiLobby : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby";

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private BackgroundStack backgroundStack { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    private bool confirmExit;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Children = new Drawable[]
                {
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () =>
                        {
                            menuMusic.StopAll();
                            startClockMusic();
                        },
                        Child = new FluXisSpriteText { Text = "Stop Music" }
                    },
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () =>
                        {
                            stopClockMusic();
                            menuMusic.GoToLayer(1, 1);
                        },
                        Child = new FluXisSpriteText { Text = "Play Prepare Music" }
                    },
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () =>
                        {
                            stopClockMusic();
                            menuMusic.GoToLayer(2, 1);
                        },
                        Child = new FluXisSpriteText { Text = "Play Win Music" }
                    },
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () =>
                        {
                            stopClockMusic();
                            menuMusic.GoToLayer(2, 1, 1);
                        },
                        Child = new FluXisSpriteText { Text = "Play Lose Music" }
                    }
                }
            }
        };
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            clock.Looping = false;
            stopClockMusic();
            backgroundStack.AddBackgroundFromMap(null);
            return false;
        }

        game.Overlay ??= new ButtonPanel
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = "Are you sure you want to exit the lobby?",
            Buttons = new[]
            {
                new ButtonData
                {
                    Text = "Leave",
                    Color = FluXisColors.ButtonRed,
                    Action = () =>
                    {
                        confirmExit = true;
                        this.Exit();
                    }
                },
                new ButtonData { Text = "Stay" }
            }
        };

        return true;
    }

    private void stopClockMusic() => clock.FadeOut(600).OnComplete(_ => clock.Stop());

    private void startClockMusic()
    {
        clock.Start();
        clock.FadeIn(600);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuMusic.StopAll();

        clock.RestartPoint = mapStore.CurrentMapSet?.Metadata.PreviewTime ?? 0;
        backgroundStack.AddBackgroundFromMap(mapStore.CurrentMapSet?.Maps[0]);
        startClockMusic();

        base.OnEntering(e);
    }
}
