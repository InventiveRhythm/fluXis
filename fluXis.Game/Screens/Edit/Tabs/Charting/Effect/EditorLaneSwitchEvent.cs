using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect.EffectEdit;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorLaneSwitchEvent : ClickableContainer
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    public MapInfo Map { get; set; }
    public LaneSwitchEvent Event { get; set; }

    private float length;
    private int count;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        Action = () =>
        {
            if (chartingContainer.BlueprintContainer.CurrentTool is SelectTool)
            {
                game.Overlay = new LaneSwtichEditorPanel
                {
                    Event = Event,
                    MapInfo = Map,
                    EditorClock = clock,
                    MapEvents = values.MapEvents
                };
            }
        };

        count = Event.Count;

        var nextEvent = values.MapEvents.LaneSwitchEvents.FirstOrDefault(e => e.Time > Event.Time);
        if (nextEvent != null)
            length = nextEvent.Time - Event.Time;
        else
            length = clock.TrackLength - Event.Time;

        if (Event.Count == Map.KeyCount)
        {
            for (int i = 0; i < Map.KeyCount; i++)
            {
                Add(new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Height = 0,
                    Width = EditorHitObjectContainer.NOTEWIDTH,
                    X = i * EditorHitObjectContainer.NOTEWIDTH,
                    Colour = Colour4.FromHex("#FF5555").Opacity(0.4f)
                });
            }
        }
        else
        {
            bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[Map.KeyCount - 2];
            bool[] current = mode[Event.Count - 1];

            for (int i = 0; i < Map.KeyCount; i++)
            {
                Add(new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Height = !current[i] ? 1 : 0,
                    Width = EditorHitObjectContainer.NOTEWIDTH,
                    X = i * EditorHitObjectContainer.NOTEWIDTH,
                    Colour = Colour4.FromHex("#FF5555").Opacity(0.4f)
                });
            }
        }
    }

    protected override void Update()
    {
        if (!values.MapEvents.LaneSwitchEvents.Contains(Event))
        {
            Expire();
            return;
        }

        var nextEvent = values.MapEvents.LaneSwitchEvents.FirstOrDefault(e => e.Time > Event.Time);
        if (nextEvent != null)
            length = nextEvent.Time - Event.Time;
        else
            length = clock.TrackLength - Event.Time;

        if (Event.Count != count)
        {
            if (Event.Count == Map.KeyCount)
            {
                for (int i = 0; i < Map.KeyCount; i++)
                {
                    var box = Children[i];
                    if (box != null)
                        box.Height = 0;
                }
            }
            else
            {
                bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[Map.KeyCount - 2];
                bool[] current = mode[Event.Count - 1];

                for (int i = 0; i < Map.KeyCount; i++)
                {
                    var box = Children[i];
                    if (box != null)
                        box.Height = !current[i] ? 1 : 0;
                }
            }

            count = Event.Count;
        }

        Height = .5f * (length * values.Zoom);
        Y = -.5f * ((Event.Time - (float)clock.CurrentTime) * values.Zoom);
    }
}
