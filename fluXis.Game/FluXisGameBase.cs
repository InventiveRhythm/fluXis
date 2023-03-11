using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Settings;
using fluXis.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game;

public partial class FluXisGameBase : osu.Framework.Game
{
    // Anything in this class is shared between the test browser and the game implementation.
    // It allows for caching global dependencies that should be accessible to tests, or changing
    // the screen scaling for all components including the test browser and framework overlays.

    private DependencyContainer dependencies;

    protected override Container<Drawable> Content => content;
    private Container content;

    public FluXisRealm Realm;
    public MapStore MapStore;
    public FluXisConfig Config;
    public BackgroundStack BackgroundStack;
    public GlobalCursorOverlay CursorOverlay;
    public SettingsMenu Settings;
    public NotificationOverlay Notifications;

    protected FluXisGameBase()
    {
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        Resources.AddStore(new DllResourceStore(FluXisResources.ResourceAssembly));
        InitFonts();

        dependencies.CacheAs(Config = new FluXisConfig(storage));
        dependencies.Cache(Realm = new FluXisRealm(storage));
        dependencies.Cache(MapStore = new MapStore(storage, Realm));
        dependencies.Cache(new BackgroundTextureStore(Host, storage));
        dependencies.Cache(BackgroundStack = new BackgroundStack());
        dependencies.Cache(CursorOverlay = new GlobalCursorOverlay());
        dependencies.Cache(Settings = new SettingsMenu());
        dependencies.Cache(Notifications = new NotificationOverlay());

        FluXisKeybindContainer keybinds;

        base.Content.Add(new SafeAreaContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1920, 1080),
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    keybinds = new FluXisKeybindContainer(this, Realm)
                }
            }
        });

        dependencies.Cache(keybinds);
    }

    protected void InitFonts()
    {
        AddFont(Resources, @"Fonts/Quicksand/Quicksand");
        AddFont(Resources, @"Fonts/Quicksand/Quicksand-SemiBold");
        AddFont(Resources, @"Fonts/Quicksand/Quicksand-Bold");
        AddFont(Resources, @"Fonts/Grade/Grade");
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
