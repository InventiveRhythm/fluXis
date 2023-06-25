using System;
using System.Linq;
using fluXis.Desktop.Integration;
using fluXis.Game;
using fluXis.Game.Integration;
using fluXis.Game.IPC;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Framework.Platform;

namespace fluXis.Desktop;

public partial class FluXisGameDesktop : FluXisGame
{
    [Resolved]
    private Storage storage { get; set; }

    private IPCImportChannel ipc;

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        var window = (SDL2DesktopWindow)host.Window;
        window.Title = "fluXis " + VersionString;
        window.ConfineMouseMode.Value = ConfineMouseMode.Never;
        window.CursorState = CursorState.Hidden;
        window.DragDrop += f => HandleDragDrop(new[] { f });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (OperatingSystem.IsWindows())
            LoadComponentAsync(new WindowsUpdateManager());

        ipc = new IPCImportChannel(Host, this);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var args = Program.Args.ToList();
        args.RemoveAll(a => a.StartsWith("-"));
        HandleDragDrop(args.ToArray());
    }

    public override LightController CreateLightController() => new OpenRGBController();
}
