using System.Linq;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorEffectContainer : Container
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private Box flashUnderlay;

    public Container<EditorFlashEvent> Flashes { get; private set; }
    public Container<EditorLaneSwitchEvent> LaneSwitches { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            flashUnderlay = new Box
            {
                Width = 14,
                RelativeSizeAxes = Axes.Y,
                Alpha = 0,
                Colour = FluXisColors.Background1,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 8 }
            },
            Flashes = new Container<EditorFlashEvent>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Y = -EditorHitObjectContainer.HITPOSITION,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 10 }
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

        settings.FlashUnderlay.BindValueChanged(val => flashUnderlay.FadeTo(val.NewValue ? 1 : 0, 200), true);
        settings.FlashUnderlayColor.BindValueChanged(val => flashUnderlay.Colour = val.NewValue, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.FlashEventAdded += AddFlash;
        map.LaneSwitchEventAdded += AddLaneSwitch;

        map.FlashEventRemoved += flash =>
        {
            var editorFlash = Flashes.FirstOrDefault(f => f.FlashEvent == flash);
            if (editorFlash != null)
                Flashes.Remove(editorFlash, true);
        };

        map.LaneSwitchEventRemoved += ls =>
        {
            var editorLs = LaneSwitches.FirstOrDefault(l => l.Event == ls);
            if (editorLs != null)
                LaneSwitches.Remove(editorLs, true);
        };
    }

    private void loadEvents()
    {
        foreach (var flashEvent in map.MapEvents.FlashEvents)
            AddFlash(flashEvent);

        foreach (var laneSwitch in map.MapEvents.LaneSwitchEvents)
            AddLaneSwitch(laneSwitch);
    }

    public void AddFlash(FlashEvent flash)
    {
        Flashes.Add(new EditorFlashEvent { FlashEvent = flash });
    }

    public void AddLaneSwitch(LaneSwitchEvent ls)
    {
        LaneSwitches.Add(new EditorLaneSwitchEvent(ls));
    }

    public void ClearAll()
    {
        Flashes.Clear();
        LaneSwitches.Clear();
    }
}
