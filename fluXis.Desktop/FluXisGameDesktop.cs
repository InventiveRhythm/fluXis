using System.Linq;
using System.Threading.Tasks;
using fluXis.Configuration;
using fluXis.Desktop.Integration;
using fluXis.Integration;
using fluXis.IPC;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace fluXis.Desktop;

public partial class FluXisGameDesktop : FluXisGame
{
    protected override bool RequiresSteam => true;

    private IPCImportChannel ipc;

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        var window = host.Window;
        window.Title = "fluXis " + VersionString;
        window.CursorState = CursorState.Hidden;
        window.DragDrop += f => Task.Run(() => HandleDragDrop(f));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        ipc = new IPCImportChannel(Host, this);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        new DiscordActivity().Initialize(this, APIClient);

        var args = Program.Args.ToList();

        if (args.Contains("--debug-join-multi"))
        {
            var idx = args.IndexOf("--debug-join-multi");
            var id = args[idx + 1];
            JoinMultiplayerRoom(id.ToIntInvariant(), "");
        }

        args.RemoveAll(a => a.StartsWith('-'));
        WaitForReady(() => HandleDragDrop(args.ToArray()));
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        ipc?.Dispose();
    }

    protected override ISteamManager CreateSteam() => new SteamManager();
    public override LightController CreateLightController() => Config.Get<bool>(FluXisSetting.OpenRGBIntegration) ? new OpenRGBController() : new LightController();
}
