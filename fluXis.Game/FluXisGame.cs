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
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Screens.Intro;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisKeybind>
{
    private static Action exitAction;

    private Container screenContainer;

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
            screenContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = Toolbar.Height },
                Children = new Drawable[]
                {
                    ScreenStack,
                }
            },
            Toolbar,
            LoginOverlay,
            ProfileOverlay,
            Settings,
            new VolumeOverlay(),
            Notifications,
            CursorOverlay
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScreenStack.Push(new IntroScreen());
        Toolbar.ScreenStack = ScreenStack;

        Fluxel.Notifications = Notifications;
        Fluxel.Connect();
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

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.ToggleSettings:
                Settings.ToggleVisibility();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    protected override void Update()
    {
        screenContainer.Padding = new MarginPadding { Top = Toolbar.Height + Toolbar.Y };
    }
}
