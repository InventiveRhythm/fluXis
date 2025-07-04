using System;
using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.Activity;
using fluXis.Online.API.Requests.Multiplayer;
using fluXis.Online.Fluxel;
using fluXis.Online.Multiplayer;
using fluXis.Overlay.Notifications;
using fluXis.Screens.Multiplayer.SubScreens.Open.List.List;
using fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.List;

public partial class MultiLobbyList : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby List";

    protected override UserActivity InitialActivity => new UserActivity.MenuGeneral();

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private MultiplayerClient client { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    private FillFlowContainer lobbyList;
    private FluXisTextFlow textFlow;
    private LoadingIcon loadingIcon;
    private MultiLobbyListFooter footer;

    // invite stuff
    private long id { get; }
    private string password { get; }

    public MultiLobbyList(long id = -1, string password = "")
    {
        this.id = id;
        this.password = password;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = lobbyList = new FillFlowContainer
                {
                    Width = 1320,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(20),
                    Direction = FillDirection.Full,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            textFlow = new FluXisTextFlow
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Width = .5f,
                WebFontSize = 20,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                TextAnchor = Anchor.TopCentre
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            footer = new MultiLobbyListFooter
            {
                BackAction = this.Exit,
                RefreshAction = loadLobbies,
                CreateAction = startCreate
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadLobbies();

        if (id > 0)
            JoinLobby(id, password);
    }

    private void loadLobbies()
    {
        loadingIcon.Show();
        textFlow.Text = "";

        lobbyList.FadeOut(FluXisScreen.FADE_DURATION).OnComplete(_ =>
        {
            lobbyList.Clear();

            var request = new MultiLobbiesRequest();

            request.Success += res =>
            {
                var lobbies = res.Data;

                foreach (var lobby in lobbies)
                    lobbyList.Add(new LobbySlot(lobby, pw => JoinLobby(lobby.RoomID, pw)));

                for (var i = 0; i < 12 - lobbies.Count; i++)
                    lobbyList.Add(new EmptyLobbySlot());

                loadingIcon.Hide();
                lobbyList.FadeIn(FluXisScreen.FADE_DURATION);
            };

            request.Failure += ex =>
            {
                loadingIcon.Hide();
                textFlow.AddParagraph("Failed to fetch lobbies.");
                textFlow.AddParagraph<FluXisSpriteText>(ex.Message, text =>
                {
                    text.WebFontSize = 14;
                    text.Alpha = .8f;
                });
            };

            api.PerformRequestAsync(request);
        });
    }

    private void startCreate()
    {
        menuMusic.StopAll();
        this.Push(new MultiSelectScreen(create));
    }

    private void create(RealmMap map, List<string> mods)
    {
        var panel = new CreateRoomPanel($"{client.Player.NameWithApostrophe} Room", async void (name, privacy, pw, cb) =>
        {
            try
            {
                await client.Create(name, privacy, pw, map.OnlineID, map.Hash);

                if (client.Room != null)
                    Schedule(() => this.Push(new MultiLobby()));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to create lobby!");
                notifications.SendError("Failed to join lobby", ex.Message);
            }
            finally { cb?.Invoke(); }
        });

        Schedule(() => panels.Content = panel);
    }

    public async void JoinLobby(long room, string password = "")
    {
        Schedule(() => panels.Content = new LoadingPanel { Text = "Joining lobby..." });

        try
        {
            await client.Join(room, password);

            if (client.Room != null)
                Schedule(() => this.Push(new MultiLobby()));
        }
        catch (Exception e)
        {
            notifications.SendError("Failed to join lobby", e.Message);
        }

        ScheduleAfterChildren(() => panels.Content.Hide());
    }

    protected override void FadeIn()
    {
        base.FadeIn();

        backgrounds.AddBackgroundFromMap(null);
        clock.VolumeOut(600).OnComplete(_ => clock.Stop());

        menuMusic.GoToLayer(0, 1);
        lobbyList.MoveToY(-100).MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        footer.Show();
    }

    protected override void FadeOut(IScreen next)
    {
        base.FadeOut(next);

        lobbyList.MoveToY(-100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        footer.Hide();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        loadLobbies();
        base.OnResuming(e);
    }
}
