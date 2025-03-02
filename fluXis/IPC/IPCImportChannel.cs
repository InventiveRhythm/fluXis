using System.Threading.Tasks;
using osu.Framework.Platform;

namespace fluXis.IPC;

public class IPCImportChannel : IpcChannel<IPCImportRequest>
{
    public IPCImportChannel(IIpcHost host, FluXisGame game = null)
        : base(host)
    {
        MessageReceived += msg =>
        {
            game?.WaitForReady(() => game.HandleDragDrop(new[] { msg.Path }));
            return null;
        };
    }

    public async Task Import(string path) => await SendMessageAsync(new IPCImportRequest { Path = path });
}

public class IPCImportRequest
{
    public string Path { get; init; }
}
