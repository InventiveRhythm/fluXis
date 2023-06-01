using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Desktop.Integration;
using fluXis.Game;
using fluXis.Game.Integration;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Framework.Platform;

namespace fluXis.Desktop;

public partial class FluXisGameDesktop : FluXisGame
{
    [Resolved]
    private Storage storage { get; set; }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        var window = (SDL2DesktopWindow)host.Window;
        window.Title = "fluXis " + VersionString;
        window.ConfineMouseMode.Value = ConfineMouseMode.Never;
        window.CursorState = CursorState.Hidden;
        window.DragDrop += f => onDragDrop(new[] { f });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (OperatingSystem.IsWindows())
            LoadComponentAsync(new WindowsUpdateManager());
    }

    private void onDragDrop(IEnumerable<string> paths) => ImportManager.ImportMultiple(paths.ToArray());

    public override LightController CreateLightController() => new OpenRGBController();
}
