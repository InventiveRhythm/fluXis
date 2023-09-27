using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Multi;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Multiplayer;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;
using WebRequest = osu.Framework.IO.Network.WebRequest;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;

public partial class MultiLobbyList : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby List";

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private LoadingPanel loadingPanel;

    private FillFlowContainer lobbyList;
    private LoadingIcon loadingIcon;

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
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        fluxel.RegisterListener<MultiplayerRoom>(EventType.MultiplayerJoinLobby, onLobbyJoin);

        loadLobbies();
    }

    private async void loadLobbies()
    {
        loadingIcon.FadeIn(200);
        lobbyList.FadeOut(200).OnComplete(_ => lobbyList.Clear());

        var request = new WebRequest($"{fluxel.Endpoint.APIUrl}/multi/lobbies");
        request.AllowInsecureRequests = true;
        await request.PerformAsync();

        var json = request.GetResponseString();
        var lobbies = JsonConvert.DeserializeObject<APIResponse<List<MultiplayerRoom>>>(json);

        foreach (var lobby in lobbies.Data)
            lobbyList.Add(new LobbySlot { Room = lobby, List = this });

        for (var i = 0; i < 12 - lobbies.Data.Count; i++)
            lobbyList.Add(new EmptyLobbySlot());

        loadingIcon.FadeOut(200);
        lobbyList.FadeIn(200);
    }

    public void JoinLobby(MultiplayerRoom room)
    {
        game.Overlay = loadingPanel = new LoadingPanel
        {
            Text = "Joining lobby...",
        };

        fluxel.SendPacketAsync(new MultiplayerJoinPacket
        {
            LobbyId = room.RoomID,
            Password = ""
        });
    }

    private void onLobbyJoin(FluxelResponse<MultiplayerRoom> res)
    {
        Schedule(() =>
        {
            if (res.Status == 200)
            {
                loadingPanel?.Hide();
                this.Push(new MultiLobby { Room = res.Data });
            }
            else
            {
                game.Overlay = null;
                notifications.SendError($"Failed to join lobby", res.Message);
            }
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 1);
        base.OnEntering(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 1);
        base.OnResuming(e);
    }
}
