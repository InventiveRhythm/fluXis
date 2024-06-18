using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

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
        Add(new EditorHitObject { Data = info });
    }

    private void remove(HitObject info)
    {
        var hitObject = InternalChildren.OfType<EditorHitObject>().FirstOrDefault(h => h.Data == info);
        if (hitObject != null) Remove(hitObject, true);
    }

    public Vector2 ScreenSpacePositionAtTime(double time, int lane) => ToScreenSpace(new Vector2(PositionFromLane(lane), PositionAtTime(time)));
    public float PositionAtTime(double time) => (float)(DrawHeight - HITPOSITION - .5f * ((time - clock.CurrentTime) * settings.Zoom));
    public float PositionFromLane(int lane) => (lane - 1) * NOTEWIDTH;

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition) => TimeAtPosition(ToLocalSpace(screenSpacePosition).Y);
    public int LaneAtScreenSpacePosition(Vector2 position) => LaneAtPosition(ToLocalSpace(position).X);
    public double TimeAtPosition(float y) => (DrawHeight - HITPOSITION - y) * 2 / settings.Zoom + clock.CurrentTime;
    public int LaneAtPosition(float x) => (int)((x + NOTEWIDTH) / NOTEWIDTH);

    public double SnapTime(double time)
    {
        var tp = map.MapInfo.GetTimingPoint(time);
        double t = tp.Time;
        double increase = tp.Signature * tp.MsPerBeat / (4 * settings.SnapDivisor);
        if (increase == 0) return time; // no snapping, the game will just freeze because it loops infinitely

        if (time < t)
        {
            while (true)
            {
                var next = t - increase;

                if (next < time)
                {
                    t = next;
                    break;
                }

                t = next;
            }
        }
        else
        {
            while (true)
            {
                var next = t + increase;
                if (next > time) break;

                t = next;
            }
        }

        if (t < 0) t = 0;
        if (t > clock.TrackLength) t = clock.TrackLength;

        return t;
    }
}
