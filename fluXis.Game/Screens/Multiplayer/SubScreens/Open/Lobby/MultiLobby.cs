using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.API.Multi;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Multiplayer;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI;
using Newtonsoft.Json;
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
    private BackgroundStack backgroundStack { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    public MultiplayerRoom Room { get; set; }

    private bool confirmExit;

    private MultiLobbyPlayerList playerList;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
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
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        fluxel.RegisterListener<MultiplayerRoomUpdate>(EventType.MultiplayerRoomUpdate, onRoomUpdate);
    }

    private void onRoomUpdate(FluxelResponse<MultiplayerRoomUpdate> response)
    {
        Schedule(() =>
        {
            string json = JsonConvert.SerializeObject(response.Data.Data);

            switch (response.Data.Type)
            {
                case "player/join":
                    var player = JsonConvert.DeserializeObject<APIUserShort>(json);
                    playerList.AddPlayer(player);
                    break;

                case "player/leave":
                    var remPlayer = int.Parse(json);
                    playerList.RemovePlayer(remPlayer);
                    break;
            }
        });
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            clock.Looping = false;
            stopClockMusic();
            backgroundStack.AddBackgroundFromMap(null);
            fluxel.SendPacketAsync(new MultiplayerLeavePacket());
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

            clock.LoadMap(mapInfo, true);
            clock.FadeOut(); // because it sets itself to 1
            clock.RestartPoint = 0;
            clock.AllowLimitedLoop = false;
            backgroundStack.AddBackgroundFromMap(mapInfo);
            startClockMusic();
        }

        base.OnEntering(e);
    }
}
