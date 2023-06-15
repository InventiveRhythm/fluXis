using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect;

public partial class EditorLaneSwitchEvent : Container
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public RealmMap Map { get; set; }
    public LaneSwitchEvent Event { get; set; }

    private float length;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        var nextEvent = values.MapEvents.LaneSwitchEvents.FirstOrDefault(e => e.Time > Event.Time);
        if (nextEvent != null)
            length = nextEvent.Time - Event.Time;
        else
            length = clock.TrackLength - Event.Time;

        if (Event.Count == Map.KeyCount)
            return; // no need to draw anything

        bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[Map.KeyCount - 2];
        bool[] current = mode[Event.Count - 1];

        for (int i = 0; i < Map.KeyCount; i++)
        {
            Add(new Box
            {
                RelativeSizeAxes = Axes.Y,
                Height = !current[i] ? 1 : 0,
                Width = EditorPlayfield.COLUMN_WIDTH,
                X = i * EditorPlayfield.COLUMN_WIDTH,
                Colour = Colour4.FromHex("#FF5555").Opacity(0.4f)
            });
        }
    }

    protected override void Update()
    {
        Height = .5f * (length * values.Zoom);
        Y = -.5f * ((Event.Time - (float)clock.CurrentTime) * values.Zoom);
    }
}
