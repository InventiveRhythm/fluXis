using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorHitObjectContainer : Container
{
    public const int HITPOSITION = 130;
    public const int NOTEWIDTH = 98;

    public IEnumerable<EditorHitObject> HitObjects => InternalChildren.OfType<EditorHitObject>();

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        map.HitObjectAdded += add;
        map.HitObjectRemoved += remove;
        map.MapInfo.HitObjects.ForEach(add);

        Add(new Box
        {
            RelativeSizeAxes = Axes.X,
            Height = 5,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Y = -HITPOSITION
        });
    }

    private void add(HitObject info)
    {
        var draw = new EditorHitObject { Data = info };
        info.EditorDrawable = draw;
        Add(draw);
    }

    private void remove(HitObject info)
    {
        var draw = info.EditorDrawable;
        if (draw == null) return;

        Remove(draw, true);
        info.EditorDrawable = null;
    }

    public Vector2 ScreenSpacePositionAtTime(double time, int lane) => ToScreenSpace(new Vector2(PositionFromLane(lane), PositionAtTime(time)));
    public float PositionAtTime(double time) => (float)(DrawHeight - HITPOSITION - .5f * ((time - clock.CurrentTime) * settings.Zoom));
    public float PositionFromLane(float lane) => (lane - 1) * NOTEWIDTH;

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition) => TimeAtPosition(ToLocalSpace(screenSpacePosition).Y);
    public int LaneAtScreenSpacePosition(Vector2 position) => LaneAtPosition(ToLocalSpace(position).X);
    public double TimeAtPosition(float y) => (DrawHeight - HITPOSITION - y) * 2 / settings.Zoom + clock.CurrentTime;
    public int LaneAtPosition(float x) => (int)((x + NOTEWIDTH) / NOTEWIDTH);
}
