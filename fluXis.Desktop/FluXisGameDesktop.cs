using System;
using System.Collections.Generic;
using System.IO;
using fluXis.Game;
using fluXis.Game.Import.FluXis;
using fluXis.Game.Import.osu;
using fluXis.Game.Import.Quaver;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Framework.Logging;
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
        ExitAction = host.Exit;
    }

    private void onDragDrop(IEnumerable<string> paths)
    {
        try
        {
            foreach (var path in paths)
            {
                switch (Path.GetExtension(path))
                {
                    case ".fms":
                        new FluXisImport(Realm, MapStore, storage, Notifications).Import(path).Start();
                        break;

                    case ".qp":
                        new QuaverImport(Realm, MapStore, storage, Notifications).Import(path).Start();
                        break;

                    case ".osz":
                        new OsuImport(Realm, MapStore, storage, Notifications).Import(path).Start();
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while importing mapset");
            Notifications.PostError("Error while importing mapset");
        }
    }
}
