using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorEffectContainer : Container
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private Box underlay;

    public Container<EditorLaneSwitchEvent> LaneSwitches { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            underlay = new Box
            {
                Width = 14,
                RelativeSizeAxes = Axes.Y,
                Alpha = 0,
                Colour = FluXisColors.Background1,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 8 }
            },
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

        settings.FlashUnderlay.BindValueChanged(val => underlay.FadeTo(val.NewValue ? 1 : 0, 200), true);
        settings.FlashUnderlayColor.BindValueChanged(val => underlay.Colour = val.NewValue, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.LaneSwitchEventAdded += AddLaneSwitch;

        map.LaneSwitchEventRemoved += ls =>
        {
            var editorLs = LaneSwitches.FirstOrDefault(l => l.Event == ls);
            if (editorLs != null)
                LaneSwitches.Remove(editorLs, true);
        };
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
