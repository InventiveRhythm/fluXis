using System.Linq;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Playfield;

public partial class EditorDesignPlayfieldManager : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private LaneSwitchManager laneSwitchManager;
    private DependencyContainer dependencies;

    public int Count { get; }

    public EditorDesignPlayfieldManager(DualMode mode)
    {
        Count = mode > DualMode.Disabled ? 2 : 1;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        dependencies.CacheAs(laneSwitchManager = new LaneSwitchManager(map.MapEvents.LaneSwitchEvents, map.RealmMap.KeyCount, map.MapInfo.NewLaneSwitchLayout)
        {
            KeepTransforms = true,
            Clock = clock
        });

        InternalChildren = new Drawable[]
        {
            laneSwitchManager,
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    Enumerable.Range(0, Count)
                              .Select(i => new EditorDesignPlayfield(i))
                              .ToArray()
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.LaneSwitchEventAdded += reloadLaneSwitches;
        map.LaneSwitchEventUpdated += reloadLaneSwitches;
        map.LaneSwitchEventRemoved += reloadLaneSwitches;

        reloadLaneSwitches(null);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        map.LaneSwitchEventAdded -= reloadLaneSwitches;
        map.LaneSwitchEventUpdated -= reloadLaneSwitches;
        map.LaneSwitchEventRemoved -= reloadLaneSwitches;
    }

    private void reloadLaneSwitches(LaneSwitchEvent _)
    {
        laneSwitchManager.Rebuild(map.MapEvents.LaneSwitchEvents, map.RealmMap.KeyCount, map.MapInfo.NewLaneSwitchLayout);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
