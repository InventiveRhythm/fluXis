using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Multiplayer;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Notification;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiplayer : FluXisTestScene
{
    private readonly TextFlowContainer textFlow;
    private readonly FillFlowContainer<Container> players;

    private string text = "";
    private bool inLobby;

    public string Text
    {
        set
        {
            text += value + "\n";
            textFlow.Text = text;
        }
    }

    public TestMultiplayer()
    {
        Fluxel.Reset();
        Fluxel.RegisterListener<int>(EventType.MultiplayerCreateLobby, onLobbyCreate);
        Fluxel.RegisterListener<APIMultiplayerLobby>(EventType.MultiplayerJoinLobby, onLobbyJoin);
        Fluxel.RegisterListener<APIMultiplayerLobby>(EventType.MultiplayerLobbyUpdate, onLobbyUpdate);

        Add(textFlow = new TextFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            TextAnchor = Anchor.TopLeft
        });

        Add(players = new FillFlowContainer<Container>
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Margin = new MarginPadding(10)
        });

        AddStep("Create Login Overlay", () =>
        {
            var notifications = new NotificationOverlay();

            Fluxel.Notifications = notifications;
            Fluxel.Connect();

            Add(new LoginOverlay());
            Add(notifications);
        });

        AddStep("Create Lobby", () =>
        {
            Text = "Creating Lobby...";
            Fluxel.SendPacket(new MultiplayerCreateLobbyPacket("Test Lobby", "password", 2));
        });
    }

    private void onLobbyCreate(FluxelResponse<int> response)
    {
        if (response.Status != 200)
        {
            Text = "Failed to create lobby!";
            inLobby = false;
            return;
        }

        Text = $"Created Lobby with ID {response.Data}";
        Text = "Joining Lobby...";
        Fluxel.SendPacket(new MultiplayerJoinLobbyPacket(response.Data, "password"));
    }

    private void onLobbyJoin(FluxelResponse<APIMultiplayerLobby> response)
    {
        Text = $"Joined Lobby [{response.Data.Name}] with ID {response.Data.ID}";
        inLobby = true;
    }

    private void onLobbyUpdate(FluxelResponse<APIMultiplayerLobby> response)
    {
        if (!inLobby) return;

        Text = "Lobby Updated!";
    }
}
