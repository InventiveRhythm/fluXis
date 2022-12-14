using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game
{
    public class FluXisGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content { get; }

        private DependencyContainer dependencies;

        protected FluXisGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1920, 1080)
            });
        }

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            Resources.AddStore(new DllResourceStore(FluXisResources.ResourceAssembly));
            InitFonts();

            GlobalKeybindContainer keybinds = new GlobalKeybindContainer();

            dependencies.Cache(new BackgroundTextureStore(Host, storage));

            base.Content.Add(keybinds);
        }

        protected void InitFonts()
        {
            AddFont(Resources, @"Fonts/Quicksand/Quicksand");
            AddFont(Resources, @"Fonts/Quicksand/Quicksand-SemiBold");
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
