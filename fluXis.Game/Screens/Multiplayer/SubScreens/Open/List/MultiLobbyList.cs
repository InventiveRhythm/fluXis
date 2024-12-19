using System;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Panel.Types;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Requests.Multiplayer;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;

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
            lobbyList = new FillFlowContainer
            {
                Width = 1320,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(20),
                Direction = FillDirection.Full,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
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
                    lobbyList.Add(new LobbySlot { Room = lobby, List = this });

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

    private void startCreate() => this.Push(new MultiSongSelect(create));

    private async void create(RealmMap map)
    {
        var panel = new LoadingPanel { Text = "Creating lobby...", };
        Schedule(() => panels.Content = panel);

        try
        {
            await client.Create($"{client.Player.NameWithApostrophe} Room", map.OnlineID, map.Hash);

            if (client.Room != null)
                Schedule(() => this.Push(new MultiLobby()));
        }
        catch (Exception e)
        {
            notifications.SendError("Failed to join lobby", e.Message);
        }

        Schedule(() => panel.Hide());
    }
}
