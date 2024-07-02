using System;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.API.Models.Multi;
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
    private LoadingIcon loadingIcon;
    private MultiLobbyListFooter footer;

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
    }

    private void loadLobbies()
    {
        loadingIcon.FadeIn(200);
        lobbyList.FadeOut(200).OnComplete(_ =>
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

                loadingIcon.FadeOut(200);
                lobbyList.FadeIn(200);
            };

            api.PerformRequestAsync(request);
        });
    }

    public async void JoinLobby(MultiplayerRoom room, string password = "")
    {
        var panel = new LoadingPanel { Text = "Joining lobby...", };
        Schedule(() => panels.Content = panel);

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

        Schedule(() => panel.Hide());
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 1);
        lobbyList.MoveToY(-100).MoveToY(0, 400, Easing.OutQuint);
        footer.Show();
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        lobbyList.MoveToY(-100, 400, Easing.OutQuint);
        footer.Hide();
        return base.OnExiting(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 1);
        clock.VolumeOut(600).OnComplete(_ => clock.Stop());
        footer.Show();
        loadLobbies();
        base.OnResuming(e);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        footer.Hide();
        menuMusic.StopAll();
        base.OnSuspending(e);
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
