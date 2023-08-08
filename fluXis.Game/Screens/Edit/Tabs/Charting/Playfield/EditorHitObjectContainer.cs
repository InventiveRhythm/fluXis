using System.Collections.Generic;
using System.Linq;
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
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        foreach (var hitObject in values.MapInfo.HitObjects)
        {
            Add(new EditorHitObject
            {
                Data = hitObject
            });
        }

        Add(new Box
        {
            RelativeSizeAxes = Axes.X,
            Height = 3,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Y = -HITPOSITION
        });
    }

    public Vector2 ScreenSpacePositionAtTime(float time, int lane) => ToScreenSpace(new Vector2(PositionFromLane(lane), PositionAtTime(time)));
    public float PositionAtTime(float time) => DrawHeight - HITPOSITION - .5f * ((time - (float)Time.Current) * values.Zoom);
    public float PositionFromLane(int lane) => (lane - 1) * NOTEWIDTH;

    public float TimeAtScreenSpacePosition(Vector2 screenSpacePosition) => TimeAtPosition(ToLocalSpace(screenSpacePosition).Y);
    public int LaneAtScreenSpacePosition(Vector2 postition) => LaneAtPosition(ToLocalSpace(postition).X);
    public float TimeAtPosition(float y) => (DrawHeight - HITPOSITION - y) * 2 / values.Zoom + (float)Time.Current;
    public int LaneAtPosition(float x) => (int)((x + NOTEWIDTH / 2f) / NOTEWIDTH);

    public float SnapTime(float time)
    {
        var tp = values.MapInfo.GetTimingPoint(time);
        float t = tp.Time;
        float increase = tp.Signature * tp.MsPerBeat / (4 * values.SnapDivisor);
        if (increase == 0) return time; // no snapping, the game will just freeze because it loops infinitely

        if (time < t)
        {
            while (true)
            {
                float next = t - increase;

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
                float next = t + increase;
                if (next > time) break;

                t = next;
            }
        }

        if (t < 0) t = 0;
        if (t > clock.TrackLength) t = clock.TrackLength;

        return t;
    }
}
