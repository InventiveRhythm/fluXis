using System.Linq;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorEffectContainer : Container
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    public Container<EditorLaneSwitchEvent> LaneSwitches { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            LaneSwitches = new Container<EditorLaneSwitchEvent>
            {
                RelativeSizeAxes = Axes.Both,
                Y = -EditorHitObjectContainer.HITPOSITION
            }
        };

        loadEvents();

        map.KeyModeChanged += _ =>
        {
            ClearAll();
            loadEvents();
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.RegisterAddListener<LaneSwitchEvent>(AddLaneSwitch);
        map.RegisterRemoveListener<LaneSwitchEvent>(ls =>
        {
            var editorLs = LaneSwitches.FirstOrDefault(l => l.Event == ls);
            if (editorLs != null)
                LaneSwitches.Remove(editorLs, true);
        });
    }

    private void loadEvents()
    {
        foreach (var laneSwitch in map.MapEvents.LaneSwitchEvents)
            AddLaneSwitch(laneSwitch);
    }

    public void AddLaneSwitch(LaneSwitchEvent ls)
    {
        LaneSwitches.Add(new EditorLaneSwitchEvent(ls));
    }

    public void ClearAll()
    {
        LaneSwitches.Clear();
    }
}
