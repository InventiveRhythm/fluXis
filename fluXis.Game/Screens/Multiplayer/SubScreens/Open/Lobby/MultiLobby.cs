using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Panel.Types;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Packets.Multiplayer;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Multiplayer.Gameplay;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiLobby : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby";

    protected override UserActivity InitialActivity => new UserActivity.MultiLobby(Room);
    public override bool AllowMusicPausing => true;

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
    private IAPIClient api { get; set; }

    [Resolved]
    private MultiplayerClient client { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    public MultiplayerRoom Room => client.Room;

    private bool hasMapDownloaded => mapStore.MapSets.Any(s => s.Maps.Any(m => m.OnlineID == Room.Map.ID));

    private bool ready;
    private bool confirmExit;

    private MultiLobbyPlayerList playerList;
    private MultiLobbyFooter footer;

    [BackgroundDependencyLoader]
    private void load()
    {
        if (Room is null)
        {
            notifications.SendError("Failed to join room!");
            Logger.Log("Failed to join room because its null!", LoggingTarget.Runtime, LogLevel.Error);
            client.LeaveRoom();
            ValidForPush = false;
            return;
        }

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
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Vertical = 100, Horizontal = 80 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 600),
                        new Dimension(GridSizeMode.Absolute, 20),
                        new Dimension()
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            playerList = new MultiLobbyPlayerList { Room = Room },
                            Empty(),
                            new MultiLobbyContainer()
                        }
                    }
                }
            },
            footer = new MultiLobbyFooter
            {
                LeaveAction = this.Exit,
                RightButtonAction = rightButtonPress,
                ChangeMapAction = changeMap
            }
        };
    }

    private void rightButtonPress()
    {
        if (hasMapDownloaded)
        {
            api.SendPacketAsync(MultiReadyPacket.CreateC2S(!ready));
            return;
        }

        mapStore.DownloadMapSet(Room.Map.MapSetID);
    }

    private void updateRightButton()
    {
        if (footer.RightButton is null)
            return;

        if (!hasMapDownloaded)
        {
            footer.RightButton.ButtonText = "Download Map";
            footer.RightButton.Icon = FontAwesome6.Solid.Download;
            return;
        }

        footer.RightButton.ButtonText = ready ? "Unready" : "Ready";
        footer.RightButton.Icon = FontAwesome6.Solid.SquareCheck;
    }

    private void mapAdded(RealmMapSet set)
    {
        if (set.OnlineID != Room.Map.MapSetID)
            return;

        Schedule(() => onMapChange(Room.Map));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (!ValidForPush)
            return;

        footer.CanChangeMap.Value = Room.Host.ID == client.Player.ID;

        client.OnDisconnect += onDisconnect;
        client.OnUserJoin += onOnUserJoin;
        client.OnUserLeave += onOnUserLeave;
        client.OnUserStateChange += updateOnUserState;
        client.OnMapChange += onMapChange;
        client.MapChangeFailed += mapChangeFailed;
        client.OnStart += startLoading;

        mapStore.MapSetAdded += mapAdded;

        updateRightButton();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        client.OnDisconnect -= onDisconnect;
        client.OnUserJoin -= onOnUserJoin;
        client.OnUserLeave -= onOnUserLeave;
        client.OnUserStateChange -= updateOnUserState;
        client.OnMapChange -= onMapChange;
        client.MapChangeFailed -= mapChangeFailed;
        client.OnStart -= startLoading;

        mapStore.MapSetAdded -= mapAdded;
    }

    private void onDisconnect()
    {
        if (IsCurrentScreen)
        {
            clock.Stop();
            panels.Content = new DisconnectedPanel(() =>
            {
                confirmExit = true;
                this.Exit();
            });
        }
    }

    private void onOnUserJoin(MultiplayerParticipant user)
    {
        if (!IsCurrentScreen)
        {
            Scheduler.AddOnce(() => onOnUserJoin(user));
            return;
        }

        RefreshActivity();
        playerList.AddPlayer(user);
    }

    private void onOnUserLeave(MultiplayerParticipant user)
    {
        if (!IsCurrentScreen)
        {
            Scheduler.AddOnce(() => onOnUserLeave(user));
            return;
        }

        RefreshActivity();
        playerList.RemovePlayer(user.ID);
    }

    private void changeMap() => MultiScreen.Push(new MultiSongSelect(map => client.ChangeMap(map.OnlineID, map.Hash)));
    private void mapChangeFailed(string error) => notifications.SendError("Map change failed", error, FontAwesome6.Solid.Bomb);

    private void onMapChange(APIMap map)
    {
        updateRightButton();

        var mapSet = mapStore.MapSets.FirstOrDefault(s => s.Maps.Any(m => m.OnlineID == map.ID));

        if (mapSet == null)
        {
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
            return;
        }

        var localMap = mapSet.Maps.FirstOrDefault(m => m.OnlineID == map.ID);
        mapStore.CurrentMap = localMap;

        clock.VolumeOut(); // because it sets itself to 1
        clock.RestartPoint = 0;
        backgrounds.AddBackgroundFromMap(localMap);
        startClockMusic();
    }

    private void updateOnUserState(long id, MultiplayerUserState state)
    {
        if (!IsCurrentScreen)
        {
            Scheduler.AddOnce(() => updateOnUserState(id, state));
            return;
        }

        var player = playerList.GetPlayer(id);
        player?.SetState(state);

        if (id != client.Player.ID)
            return;

        ready = state == MultiplayerUserState.Ready;
        updateRightButton();
    }

    private void startLoading()
    {
        var map = mapStore.CurrentMapSet.Maps.FirstOrDefault(x => x.OnlineID == Room.Map.ID);

        if (map == null)
        {
            notifications.SendError("Failed to find map locally.");
            api.SendPacketAsync(MultiReadyPacket.CreateC2S(false));
            return;
        }

        var mods = new List<IMod>();

        MultiScreen.Push(new GameplayLoader(map, mods, () => new MultiGameplayScreen(client, map, mods) { Scores = client.Room.Scores }));
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            if (Room is not null)
                client.LeaveRoom();

            clock.Looping = false;
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
            footer.Hide();
            return base.OnExiting(e);
        }

        panels.Content ??= new ButtonPanel
        {
            Icon = FontAwesome6.Solid.DoorOpen,
            Text = "Are you sure you want to exit the lobby?",
            Buttons = new ButtonData[]
            {
                new DangerButtonData(LocalizationStrings.General.PanelGenericConfirm, () =>
                {
                    confirmExit = true;
                    this.Exit();
                }),
                new CancelButtonData()
            }
        };

        return true;
    }

    private void stopClockMusic() => clock.VolumeOut(600).OnComplete(_ => clock.Stop());

    private void startClockMusic()
    {
        clock.Start();
        clock.VolumeIn(600);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        if (Room is null)
        {
            confirmExit = true;
            this.Exit();
            return;
        }

        base.OnResuming(e);
        onMapChange(Room.Map);
        api.SendPacketAsync(MultiReadyPacket.CreateC2S(false));
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

            clock.VolumeOut(); // because it sets itself to 1
            clock.RestartPoint = 0;
            backgrounds.AddBackgroundFromMap(mapInfo);
            startClockMusic();
        }

        footer.Show();
        base.OnEntering(e);
    }
}
