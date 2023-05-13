using System;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Screens.Intro;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;

namespace fluXis.Game;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisKeybind>
{
    public static readonly string[] AUDIO_EXTENSIONS = { ".mp3", ".wav", ".ogg" };
    public static readonly string[] MAP_EXTENSIONS = { ".fms", ".osz", ".qp" };
    public static readonly string[] IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png" };
    public static readonly string[] VIDEO_EXTENSIONS = { ".mp4", ".mov", ".avi", ".flv", ".mpg", ".wmv", ".m4v" };

    protected static Action ExitAction;

    private Container screenContainer;

    private Storage storage;

    public bool Sex = true;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        this.storage = storage;

        Discord.Init();

        Children = new Drawable[]
        {
            Conductor,
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

    public static void ExitGame()
    {
        ExitAction?.Invoke();
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
