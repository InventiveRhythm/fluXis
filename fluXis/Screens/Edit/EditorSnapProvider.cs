using System;

namespace fluXis.Screens.Edit;

public class EditorSnapProvider
{
    private EditorMap map { get; }
    private EditorSettings settings { get; }
    private EditorClock clock { get; }

    public float CurrentStep => map.MapInfo.GetTimingPoint(clock.CurrentTime).MsPerBeat / settings.SnapDivisor;

    public EditorSnapProvider(EditorMap map, EditorSettings settings, EditorClock clock)
    {
        this.map = map;
        this.settings = settings;
        this.clock = clock;
    }

    public double SnapTime(double time) => SnapTime(time, false);
    public double SnapTime(double time, int snap) => SnapTime(time, false, snap);

    public double SnapTime(double time, bool allowNext, int snap = -1)
    {
        // fix for a weird bug that tries to snap a 17-digit number
        if (time >= clock.TrackLength)
            return time;

        if (snap <= 0) snap = settings.SnapDivisor;

        var tp = map.MapInfo.GetTimingPoint(time);
        var t = tp.Time;
        double increase = tp.MsPerBeat / snap;
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

                if (next > time)
                {
                    if (!allowNext)
                        break;

                    // if the next snap is closer,
                    // then use it
                    var diffN = Math.Abs(time - next);

                    if (diffN / increase < .3f)
                        t = next;

                    break;
                }

                t = next;
            }
        }

        if (t < 0) t = 0;
        if (t > clock.TrackLength) t = clock.TrackLength;

        return t;
    }
}
