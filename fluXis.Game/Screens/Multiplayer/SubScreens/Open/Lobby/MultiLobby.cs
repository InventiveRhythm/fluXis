using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Multiplayer;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Multiplayer.Gameplay;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

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
    private GlobalBackground backgrounds { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [Resolved]
    private MultiplayerScreen multiScreen { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    public MultiplayerRoom Room { get; set; }

    private MultiplayerClient client;
    private bool ready;

    private bool confirmExit;

    private MultiLobbyPlayerList playerList;
    private CornerButton readyButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            client = new OnlineMultiplayerClient { Room = Room },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding(30),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = Room.Settings.Name,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        FontSize = 30
                    },
                    new FluXisSpriteText
                    {
                        Text = $"hosted by {Room.Host.Username}",
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        FontSize = 20
                    }
                }
            },
            new GridContainer
            {
                Width = 1600,
                Height = 800,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 20),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 20),
                    new Dimension()
                },
                RowDimensions = new[]
                {
                    new Dimension()
                },
                Content = new[]
                {
                    new[]
                    {
                        playerList = new MultiLobbyPlayerList { Room = Room },
                        Empty(),
                        new MultiLobbyContainer(),
                        Empty(),
                        new MultiLobbyContainer(),
                    }
                }
            },
            readyButton = new CornerButton
            {
                Corner = Corner.BottomRight,
                ButtonText = "Ready",
                Action = () =>
                {
                    ready = !ready;
                    fluxel.SendPacketAsync(new MultiplayerReadyPacket(ready));
                    readyButton.ButtonText = ready ? "Unready" : "Ready";
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        client.UserJoined += user => playerList.AddPlayer(user);
        client.UserLeft += user => playerList.RemovePlayer(user.ID);
        client.ReadyStateChanged += updateReadyState;
        client.Starting += startLoading;
    }

    private void updateReadyState(long id, bool ready)
    {
        var player = playerList.GetPlayer(id);
        player?.SetReady(ready);
    }

    private void startLoading()
    {
        var map = mapStore.CurrentMapSet.Maps.First();
        var mods = new List<IMod>();

        multiScreen.Push(new GameplayLoader(map, mods, () => new MultiGameplayScreen(map, mods)));
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            clock.Looping = false;
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
            fluxel.SendPacketAsync(new MultiplayerLeavePacket());
            readyButton.Hide();
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

        var map = mapStore.MapSets.FirstOrDefault(s => s.Maps.Any(m => m.OnlineID == Room.Maps[0].Id));

        if (map != null)
        {
            mapStore.CurrentMapSet = map;

            var mapInfo = map.Maps.FirstOrDefault(m => m.OnlineID == Room.Maps[0].Id);
            if (mapInfo == null) return; // what

            clock.FadeOut(); // because it sets itself to 1
            clock.RestartPoint = 0;
            clock.AllowLimitedLoop = false;
            backgrounds.AddBackgroundFromMap(mapInfo);
            startClockMusic();
        }

        readyButton.Show();
        base.OnEntering(e);
    }
}
