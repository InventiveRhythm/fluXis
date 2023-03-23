using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Multiplayer;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Notification;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiplayer : FluXisTestScene
{
    public TestMultiplayer()
    {
        Fluxel.Reset();
        Fluxel.RegisterListener<int>(EventType.MultiplayerCreateLobby, onLobbyCreate);
        Fluxel.RegisterListener<APIMultiplayerLobby>(EventType.MultiplayerJoinLobby, onLobbyJoin);

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
            Fluxel.SendPacket(new MultiplayerCreateLobbyPacket("Test Lobby", "password", 2));
        });
    }

    private void onLobbyCreate(FluxelResponse<int> response)
    {
        Fluxel.SendPacket(new MultiplayerJoinLobbyPacket(response.Data, "password"));
    }

    private void onLobbyJoin(FluxelResponse<APIMultiplayerLobby> response)
    {
        // Logger.Log($"Joined lobby {response.Data.id}!");
    }
}
