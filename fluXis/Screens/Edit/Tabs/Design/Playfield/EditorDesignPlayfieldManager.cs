using System.Linq;
using fluXis.Map.Structures.Events;
using fluXis.Online.API.Models.Maps;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Design.Playfield;

public partial class EditorDesignPlayfieldManager : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private LaneSwitchManager laneSwitchManager;
    private BeatPulseManager beatPulseManager;
    private DependencyContainer dependencies;

    private Drawable pulseDrawable { get; }

    public int Count { get; }

    public EditorDesignPlayfieldManager(DualMode mode, Drawable pulseDrawable)
    {
        this.pulseDrawable = pulseDrawable;

        Count = mode > DualMode.Disabled ? 2 : 1;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        dependencies.CacheAs(laneSwitchManager = new LaneSwitchManager(map.MapEvents.LaneSwitchEvents, map.RealmMap.KeyCount, map.MapInfo.NewLaneSwitchLayout)
        {
            KeepTransforms = true,
            Clock = clock
        });

        InternalChildren = new Drawable[]
        {
            beatPulseManager = new BeatPulseManager(map.MapInfo, map.MapEvents.BeatPulseEvents, pulseDrawable) { Clock = clock },
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

        map.BeatPulseEventAdded += reloadBeatPulses;
        map.BeatPulseEventUpdated += reloadBeatPulses;
        map.BeatPulseEventRemoved += reloadBeatPulses;

        reloadLaneSwitches(null);
        reloadBeatPulses(null);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        map.LaneSwitchEventAdded -= reloadLaneSwitches;
        map.LaneSwitchEventUpdated -= reloadLaneSwitches;
        map.LaneSwitchEventRemoved -= reloadLaneSwitches;

        map.BeatPulseEventAdded -= reloadBeatPulses;
        map.BeatPulseEventUpdated -= reloadBeatPulses;
        map.BeatPulseEventRemoved -= reloadBeatPulses;
    }

    private void reloadLaneSwitches(LaneSwitchEvent _)
        => laneSwitchManager.Rebuild(map.MapEvents.LaneSwitchEvents, map.RealmMap.KeyCount, map.MapInfo.NewLaneSwitchLayout);

    private void reloadBeatPulses(BeatPulseEvent _)
        => beatPulseManager.Rebuild(map.MapEvents.BeatPulseEvents);

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
