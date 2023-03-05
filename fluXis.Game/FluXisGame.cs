using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using fluXis.Game.Audio;
using fluXis.Game.Import.FluXis;
using fluXis.Game.Import.osu;
using fluXis.Game.Import.Quaver;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Overlay;
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisKeybind>
{
    private static Action exitAction;

    private ScreenStack screenStack;
    private OnlineOverlay overlay;

    private Storage storage;

    public bool Sex = true;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        this.storage = storage;

        Discord.Init();

        // Add your top-level game components here.
        // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
        Children = new Drawable[]
        {
            new Conductor(),
            BackgroundStack,
            screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
            overlay = new OnlineOverlay(),
            Settings,
            new VolumeOverlay(),
            CursorOverlay
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Fluxel.Connect();
        screenStack.Push(new MenuScreen());
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        Version version = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();

        var window = (SDL2DesktopWindow)host.Window;
        window.Title = "fluXis v" + version;
        window.ConfineMouseMode.Value = ConfineMouseMode.Never;
        window.CursorState = CursorState.Hidden;
        window.DragDrop += f => onDragDrop(new[] { f });
        exitAction = Host.Exit;
    }

    public static void ExitGame()
    {
        exitAction?.Invoke();
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
                        new FluXisImport(Realm, MapStore, storage).Import(path).Start();
                        break;

                    case ".qp":
                        new QuaverImport(Realm, MapStore, storage).Import(path).Start();
                        break;

                    case ".osz":
                        new OsuImport(Realm, MapStore, storage).Import(path).Start();
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while importing mapset");
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.ToggleOnlineOverlay:
                overlay.ToggleVisibility();
                return true;

            case FluXisKeybind.ToggleSettings:
                Settings.ToggleVisibility();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
}
