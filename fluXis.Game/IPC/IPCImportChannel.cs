using System.Threading.Tasks;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.IPC;

public class IPCImportChannel : IpcChannel<IPCImportRequest>
{
    public IPCImportChannel(IIpcHost host, FluXisGame game = null)
        : base(host)
    {
        MessageReceived += msg =>
        {
            Logger.Log($"IPCImportChannel: {msg.Path}", LoggingTarget.Runtime, LogLevel.Important);

            game?.HandleDragDrop(new[] { msg.Path });
            return null;
        };
    }

    public async Task Import(string path)
    {
        Logger.Log($"IPCImportChannel: Import({path})", LoggingTarget.Runtime, LogLevel.Important);
        await SendMessageAsync(new IPCImportRequest { Path = path });
    }
}

public class IPCImportRequest
{
    public string Path { get; set; }
}
