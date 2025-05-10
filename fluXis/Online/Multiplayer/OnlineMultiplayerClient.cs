using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using fluXis.Scoring;
using Midori.Networking.WebSockets;
using Midori.Networking.WebSockets.Frame;
using Midori.Networking.WebSockets.Typed;
using osu.Framework.Allocation;
using osu.Framework.Logging;

namespace fluXis.Online.Multiplayer;

public partial class OnlineMultiplayerClient : MultiplayerClient
{
    [Resolved]
    private IAPIClient api { get; set; }

    public override bool Connected => connection?.State == WebSocketState.Open;
    public override APIUser Player => api.User.Value;

    private TypedWebSocketClient<IMultiplayerServer, IMultiplayerClient> connection = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        try
        {
            connection = api.GetWebSocket<IMultiplayerServer, IMultiplayerClient>(this, "/multiplayer");
            connection.OnClose += TriggerDisconnect;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to connect to multiplayer server!");
            TriggerConnectionError(ex);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        connection?.Close(WebSocketCloseCode.NormalClosure, "Disconnecting.");
        connection?.Dispose();
    }

    protected override async Task<MultiplayerRoom> CreateRoom(string name, MultiplayerPrivacy privacy, string password, long mapid, string hash)
        => await connection.Server.CreateRoom(name, privacy, password, mapid, hash);

    protected override async Task<MultiplayerRoom> JoinRoom(long id, string password)
        => await connection.Server.JoinRoom(id, password);

    public override async Task LeaveRoom()
    {
        await connection.Server.LeaveRoom();
        Room = null;
    }

    public override async Task ChangeMap(long map, string hash, List<string> mods)
    {
        var result = await connection.Server.UpdateMap(map, hash, mods);

        if (!result)
            throw new Exception("Failed to update map.");
    }

    public override async Task TransferHost(long target) => await connection.Server.TransferHost(target);
    public override async Task UpdateScore(int score) => await connection.Server.UpdateScore(score);
    public override async Task Finish(ScoreInfo score) => await connection.Server.FinishPlay(score);
    public override async Task SetReadyState(bool ready) => await connection.Server.UpdateReadyState(ready);

    public override string ToString() => $"{connection.State}";
}
