using System.Linq;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorLaneSwitchEvent : ClickableContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    public LaneSwitchEvent Event { get; set; }

    private float length;
    private int count;

    [BackgroundDependencyLoader]
    private void load(PointsSidebar points)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        Action = () => points.ShowPoint(Event);

        count = Event.Count;

        var nextEvent = map.MapEvents.LaneSwitchEvents.FirstOrDefault(e => e.Time > Event.Time);
        length = (nextEvent?.Time ?? clock.TrackLength) - Event.Time;

        if (Event.Count >= map.RealmMap.KeyCount)
        {
            for (int i = 0; i < map.RealmMap.KeyCount; i++)
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
            bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[map.RealmMap.KeyCount - 2];
            bool[] current = mode[Event.Count - 1];

            for (int i = 0; i < map.RealmMap.KeyCount; i++)
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
        if (!map.MapEvents.LaneSwitchEvents.Contains(Event))
        {
            Expire();
            return;
        }

        var nextEvent = map.MapEvents.LaneSwitchEvents.FirstOrDefault(e => e.Time > Event.Time);
        if (nextEvent != null)
            length = nextEvent.Time - Event.Time;
        else
            length = clock.TrackLength - Event.Time;

        if (Event.Count != count)
        {
            if (Event.Count == map.RealmMap.KeyCount)
            {
                for (int i = 0; i < map.RealmMap.KeyCount; i++)
                {
                    var box = Children[i];
                    if (box != null)
                        box.Height = 0;
                }
            }
            else
            {
                bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[map.RealmMap.KeyCount - 2];
                bool[] current = mode[Event.Count - 1];

                for (int i = 0; i < map.RealmMap.KeyCount; i++)
                {
                    var box = Children[i];
                    if (box != null)
                        box.Height = !current[i] ? 1 : 0;
                }
            }

            count = Event.Count;
        }

        Height = .5f * (length * settings.Zoom);
        Y = -.5f * ((Event.Time - (float)clock.CurrentTime) * settings.Zoom);
    }
}
