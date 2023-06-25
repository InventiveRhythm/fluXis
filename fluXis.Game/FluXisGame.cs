using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Overlay.FPS;
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Screens.Intro;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Game;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisKeybind>
{
    public static readonly string[] AUDIO_EXTENSIONS = { ".mp3", ".wav", ".ogg" };
    public static readonly string[] MAP_EXTENSIONS = { ".fms", ".osz", ".qp" };
    public static readonly string[] IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png" };
    public static readonly string[] VIDEO_EXTENSIONS = { ".mp4", ".mov", ".avi", ".flv", ".mpg", ".wmv", ".m4v" };

    private Container screenContainer;
    private Container exitContainer;
    private FluXisSpriteText seeyaText;

    public bool Sex = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        Discord.Init(Fluxel);

        Children = new Drawable[]
        {
            AudioClock,
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
            LoginOverlay,
            Toolbar,
            RegisterOverlay,
            ChatOverlay,
            ProfileOverlay,
            Settings,
            new VolumeOverlay(),
            Notifications,
            new FpsOverlay(),
            CursorOverlay,
            exitContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black
                    },
                    seeyaText = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "Goodbye!",
                        FontSize = 32
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScreenStack.Push(new IntroScreen());
        Toolbar.ScreenStack = ScreenStack;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.ToggleSettings:
                Settings.ToggleVisibility();
                return true;

            case FluXisKeybind.OpenSkinEditor:
                OpenSkinEditor();
                return true;
        }

        if (ScreenStack.AllowMusicControl)
        {
            switch (e.Action)
            {
                case FluXisKeybind.MusicPause:
                    if (AudioClock.IsRunning) AudioClock.Stop();
                    else AudioClock.Start();
                    return true;

                case FluXisKeybind.MusicPrevious:
                    PreviousSong();
                    return true;

                case FluXisKeybind.MusicNext:
                    NextSong();
                    return true;
            }
        }

        return false;
    }

    public void HandleDragDrop(IEnumerable<string> files) => ImportManager.ImportMultiple(files.ToArray());

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    protected override void Update()
    {
        screenContainer.Padding = new MarginPadding { Top = Toolbar.Height + Toolbar.Y };
    }

    public override void Exit()
    {
        CursorOverlay.FadeOut(600);
        Toolbar.ShowToolbar.Value = false;
        AudioClock.FadeOut(1500);
        exitContainer.FadeIn(1000).OnComplete(_ => seeyaText.FadeOut(1000).OnComplete(_ => base.Exit()));
    }
}
