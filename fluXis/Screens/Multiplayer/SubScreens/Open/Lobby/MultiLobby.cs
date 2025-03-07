using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.Activity;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.Multiplayer;
using fluXis.Overlay.Notifications;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Multiplayer.Gameplay;
using fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI;
using fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI.Disc;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;

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
    private MultiplayerClient client { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private FluXisGame game { get; set; }

    public MultiplayerRoom Room => client.Room;

    private bool hasMapDownloaded => mapStore.MapSets.Any(s => s.Maps.Any(m => m.OnlineID == Room.Map.ID));

    private bool ready;
    private bool confirmExit;

    private List<IMod> mods = new();

    private FluXisSpriteText hostText;
    private MultiLobbyPlayerList playerList;
    private MultiLobbyDisc disc;
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
                    hostText = new FluXisSpriteText
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
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension()
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            playerList = new MultiLobbyPlayerList { Room = Room },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Size = new Vector2(1.5f),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Child = disc = new MultiLobbyDisc()
                                }
                            }
                        }
                    }
                }
            },
            footer = new MultiLobbyFooter
            {
                LeaveAction = this.Exit,
                RightButtonAction = rightButtonPress,
                ChangeMapAction = changeMap,
                ViewMapAction = () => game.PresentMapSet(Room.Map.MapSetID)
            }
        };
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
        client.OnHostChange += hostChanged;
        client.OnUserStateChange += updateOnUserState;
        client.OnMapChange += onMapChange;
        client.OnStart += startLoading;

        mapStore.MapSetAdded += mapAdded;

        updateRightButton();
    }

    private void rightButtonPress()
    {
        if (hasMapDownloaded)
        {
            client.SetReadyState(!ready);
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

        Schedule(() => onMapChange(Room.Map, Room.Mods));
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        client.OnDisconnect -= onDisconnect;
        client.OnUserJoin -= onOnUserJoin;
        client.OnUserLeave -= onOnUserLeave;
        client.OnHostChange -= hostChanged;
        client.OnUserStateChange -= updateOnUserState;
        client.OnMapChange -= onMapChange;
        client.OnStart -= startLoading;

        mapStore.MapSetAdded -= mapAdded;
    }

    private void onDisconnect()
    {
        confirmExit = true;

        if (IsCurrentScreen)
            clock.Stop();
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

    private void changeMap() => this.Push(new MultiSelectScreen(async void (map, mods) =>
    {
        try
        {
            await client.ChangeMap(map.OnlineID, map.Hash, mods);
        }
        catch (Exception ex)
        {
            notifications.SendError("Map change failed", ex.Message, FontAwesome6.Solid.Bomb);
        }
    }));

    private void onMapChange(APIMap map, List<string> modsString)
    {
        updateRightButton();

        mods = modsString.Select(ModUtils.GetFromAcronym).Where(m => m != null).ToList();
        disc.UpdateMods(mods);

        var mapSet = mapStore.MapSets.FirstOrDefault(s => s.Maps.Any(m => m.OnlineID == map.ID));

        if (mapSet == null)
        {
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);

            var dummy = RealmMap.CreateNew();
            dummy.Metadata.Title = map.Title;
            dummy.Metadata.TitleRomanized = map.TitleRomanized;
            dummy.Metadata.Artist = map.Artist;
            dummy.Metadata.ArtistRomanized = map.ArtistRomanized;
            dummy.Difficulty = map.Difficulty;
            dummy.Filters.NotesPerSecond = (float)map.NotesPerSecond;
            dummy.KeyCount = map.Mode;
            dummy.Rating = (float)map.Rating;
            dummy.Hash = "dummy";
            mapStore.CurrentMap = dummy;

            return;
        }

        var localMap = mapSet.Maps.FirstOrDefault(m => m.OnlineID == map.ID);
        var change = localMap?.ID != mapStore.CurrentMap.ID;

        if (change)
        {
            mapStore.CurrentMap = localMap;
            clock.VolumeOut(); // because it sets itself to 1
        }

        clock.RestartPoint = 0;
        backgrounds.AddBackgroundFromMap(localMap);
        startClockMusic();
        clock.Seek(Math.Max(localMap?.Metadata.PreviewTime ?? 0, 0));
        clock.RestartPoint = 0;
        clock.AllowLimitedLoop = false;

        var rateMod = mods.OfType<RateMod>().FirstOrDefault();
        clock.RateTo(rateMod?.Rate ?? 1, 400);
    }

    private void hostChanged(long newHost)
    {
        var isHost = newHost == client.Player.ID;
        footer.CanChangeMap.Value = isHost;
        hostText.Text = $"hosted by {Room.Host.Username}";

        if (isHost)
            notifications.SendText("You are now the lobby host.");
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
            client.SetReadyState(false);
            return;
        }

        MultiScreen.Push(new GameplayLoader(map, mods, () => new MultiGameplayScreen(client, map, mods) { Scores = client.Room.Scores }));
    }

    private void stopClockMusic() => clock.VolumeOut(FluXisScreen.MOVE_DURATION).OnComplete(_ => clock.Stop());

    private void startClockMusic()
    {
        clock.Start();
        clock.VolumeIn(FluXisScreen.MOVE_DURATION);
    }

    protected override void FadeIn()
    {
        base.FadeIn();
        footer.Show();
        disc.Show();
    }

    protected override void FadeOut(IScreen next)
    {
        base.FadeOut(next);
        footer.Hide();
        disc.Hide();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuMusic.StopAll();

        onMapChange(Room.Map, Room.Mods);
        base.OnEntering(e);
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
        onMapChange(Room.Map, Room.Mods);
        client.SetReadyState(false);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit)
        {
            if (Room is not null)
                client.LeaveRoom();

            if (mapStore.CurrentMap.Hash == "dummy")
                mapStore.CurrentMapSet = mapStore.GetRandom();

            clock.Looping = false;
            stopClockMusic();
            backgrounds.AddBackgroundFromMap(null);
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
}
