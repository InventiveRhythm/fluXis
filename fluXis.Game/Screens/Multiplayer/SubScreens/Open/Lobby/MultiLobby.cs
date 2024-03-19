using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Multiplayer.Gameplay;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI;
using fluXis.Game.UI;
using fluXis.Shared.API.Packets.Multiplayer;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiLobby : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby";

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [Resolved]
    private MultiplayerScreen multiScreen { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private MultiplayerClient client { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    public MultiplayerRoom Room => client.Room;

    private bool ready;

    private bool confirmExit;

    private MultiLobbyPlayerList playerList;
    private CornerButton readyButton;

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
                Action = toggleReadyState
            }
        };
    }

    private void toggleReadyState()
    {
        ready = !ready;
        fluxel.SendPacketAsync(MultiReadyPacket.CreateC2S(ready));
        readyButton.ButtonText = ready ? "Unready" : "Ready";
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        client.UserJoined += onUserJoined;
        client.UserLeft += onUserLeft;
        client.MapChanged += mapChanged;
        client.MapChangedFailed += mapChangeFailed;
        client.ReadyStateChanged += updateReadyState;
        client.Starting += startLoading;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        client.UserJoined -= onUserJoined;
        client.UserLeft -= onUserLeft;
        client.MapChanged -= mapChanged;
        client.MapChangedFailed -= mapChangeFailed;
        client.ReadyStateChanged -= updateReadyState;
        client.Starting -= startLoading;
    }

    private void onUserJoined(APIUserShort user)
    {
        playerList.AddPlayer(user);
    }

    private void onUserLeft(APIUserShort user)
    {
        playerList.RemovePlayer(user.ID);
    }

    private void mapChanged(IAPIMapShort map)
    {
        var mapSet = mapStore.MapSets.FirstOrDefault(s => s.Maps.Any(m => m.OnlineID == map.ID));

        if (mapSet == null)
        {
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
            return;
        }

        var localMap = mapSet.Maps.FirstOrDefault(m => m.OnlineID == map.ID);
        mapStore.CurrentMap = localMap;

        clock.FadeOut(); // because it sets itself to 1
        clock.RestartPoint = 0;
        clock.AllowLimitedLoop = false;
        backgrounds.AddBackgroundFromMap(localMap);
        startClockMusic();
    }

    private void mapChangeFailed(string error)
    {
        notifications.SendError("Map change failed", error, FontAwesome6.Solid.Bomb);
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

        multiScreen.Push(new GameplayLoader(map, mods, () => new MultiGameplayScreen(client, map, mods)));
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.C:
                multiScreen.Push(new MultiSongSelect(map => client.ChangeMap(map.OnlineID, map.Hash)));
                return true;
        }

        return false;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            clock.Looping = false;
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
            client.LeaveRoom();
            fluxel.SendPacketAsync(new MultiLeavePacket());
            readyButton.Hide();
            return false;
        }

        panels.Content ??= new ButtonPanel
        {
            Icon = FontAwesome6.Solid.DoorOpen,
            Text = "Are you sure you want to exit the lobby?",
            Buttons = new ButtonData[]
            {
                new DangerButtonData(ButtonPanel.COMMON_CONFIRM, () =>
                {
                    confirmExit = true;
                    this.Exit();
                }),
                new CancelButtonData(ButtonPanel.COMMON_CANCEL)
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

        var map = mapStore.MapSets.FirstOrDefault(s => s.Maps.Any(m => m.OnlineID == Room.Map.ID));

        if (map != null)
        {
            mapStore.CurrentMapSet = map;

            var mapInfo = map.Maps.FirstOrDefault(m => m.OnlineID == Room.Map.ID);
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
