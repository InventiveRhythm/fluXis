using System;
using System.Reflection;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Overlay;
using fluXis.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game
{
    public class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisKeybind>
    {
        private ScreenStack screenStack;
        private OnlineOverlay overlay;

        [Cached]
        private MapStore mapStore = new();

        [Cached]
        private BackgroundStack backgroundStack = new();

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            Discord.Init();

            mapStore.LoadMaps(storage);

            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Children = new Drawable[]
            {
                backgroundStack,
                screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                overlay = new OnlineOverlay()
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(new Conductor());
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
            // window.CursorState = CursorState.Hidden;
            window.DragDrop += f => onDragDrop(new[] { f });
        }

        private void onDragDrop(string[] paths)
        {
            foreach (var path in paths)
            {
                Logger.Log($"DragDrop: {path}");
            }
        }

        public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
        {
            switch (e.Action)
            {
                case FluXisKeybind.ToggleOnlineOverlay:
                    overlay.ToggleVisibility();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
    }
}
