using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Multiplayer.Gameplay;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI;
using fluXis.Shared.API.Packets.Multiplayer;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Multi;
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
    private FluxelClient fluxel { get; set; }

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
            fluxel.SendPacketAsync(MultiReadyPacket.CreateC2S(!ready));
            return;
        }

        mapStore.DownloadMapSet(Room.Map.MapSetID);
    }

    private void updateRightButton()
    {
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

        mapChanged(Room.Map);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        footer.CanChangeMap.Value = Room.Host.ID == client.Player.ID;

        client.UserJoined += onUserJoined;
        client.UserLeft += onUserLeft;
        client.UserStateChanged += updateUserState;
        client.MapChanged += mapChanged;
        client.MapChangedFailed += mapChangeFailed;
        client.Starting += startLoading;

        mapStore.MapSetAdded += mapAdded;

        updateRightButton();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        client.UserJoined -= onUserJoined;
        client.UserLeft -= onUserLeft;
        client.UserStateChanged -= updateUserState;
        client.MapChanged -= mapChanged;
        client.MapChangedFailed -= mapChangeFailed;
        client.Starting -= startLoading;

        mapStore.MapSetAdded -= mapAdded;
    }

    private void onUserJoined(MultiplayerParticipant user)
    {
        if (!IsCurrentScreen)
        {
            Scheduler.AddOnce(() => onUserJoined(user));
            return;
        }

        playerList.AddPlayer(user);
    }

    private void onUserLeft(MultiplayerParticipant user)
    {
        if (!IsCurrentScreen)
        {
            Scheduler.AddOnce(() => onUserLeft(user));
            return;
        }

        playerList.RemovePlayer(user.ID);
    }

    private void changeMap() => MultiScreen.Push(new MultiSongSelect(map => client.ChangeMap(map.OnlineID, map.Hash)));
    private void mapChangeFailed(string error) => notifications.SendError("Map change failed", error, FontAwesome6.Solid.Bomb);

    private void mapChanged(APIMap map)
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
        clock.AllowLimitedLoop = false;
        backgrounds.AddBackgroundFromMap(localMap);
        startClockMusic();
    }

    private void updateUserState(long id, MultiplayerUserState state)
    {
        if (!IsCurrentScreen)
        {
            Scheduler.AddOnce(() => updateUserState(id, state));
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
            fluxel.SendPacketAsync(MultiReadyPacket.CreateC2S(false));
            return;
        }

        var mods = new List<IMod>();

        MultiScreen.Push(new GameplayLoader(map, mods, () => new MultiGameplayScreen(client, map, mods)));
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            clock.Looping = false;
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
            client.LeaveRoom();
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
        base.OnResuming(e);
        mapChanged(Room.Map);
        fluxel.SendPacketAsync(MultiReadyPacket.CreateC2S(false));
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
            clock.AllowLimitedLoop = false;
            backgrounds.AddBackgroundFromMap(mapInfo);
            startClockMusic();
        }

        footer.Show();
        base.OnEntering(e);
    }
}
