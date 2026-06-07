using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Online.Fluxel;
using fluXis.Online.Spectator.Models;
using Midori.Networking.WebSockets.Frame;
using Midori.Networking.WebSockets.Typed;
using osu.Framework.Allocation;
using osu.Framework.Logging;

namespace fluXis.Online.Spectator;

public partial class OnlineSpectatorClient : SpectatorClient
{
    [Resolved]
    private IAPIClient api { get; set; }

    private TypedWebSocketClient<ISpectatorServer, ISpectatorClient> connection = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        queueConnect();
    }

    private void queueConnect() => Scheduler.AddDelayed(connect, 2000);

    private void connect() => Task.Run(() =>
    {
        if (IsDisposed)
            return;

        try
        {
            connection = api.GetWebSocket<ISpectatorServer, ISpectatorClient>(this, "/spectator");
            connection.OnClose += TriggerDisconnect;
            Logger.Log("Connected to spectator server!", LoggingTarget.Network);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to connect to spectator server!", LoggingTarget.Network);
            queueConnect();
        }
    });

    protected override void TriggerDisconnect()
    {
        base.TriggerDisconnect();
        queueConnect();
    }

    protected override void Dispose(bool isDisposing)
    {
        connection?.Close(WebSocketCloseCode.NormalClosure, "Disconnecting.");
        connection?.Dispose();

        base.Dispose(isDisposing);
    }

    protected override void StartPlayingCore(long mapid, IEnumerable<string> mods)
        => connection?.Server.StartSession(new SpectatorState { MapID = mapid, Mods = mods.ToList() });

    protected override void SendBundle(SpectatorFrameBundle bundle)
        => connection?.Server.SendFrameBundle(bundle);

    public override void StopPlaying()
        => connection?.Server.EndSession();

    public override Task StartWatching(long id)
        => connection?.Server.StartWatching(id);

    public override Task StopWatching(long id)
        => connection?.Server.StopWatching(id);
}
