using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Lines;

public partial class EditorTimingLines : Container<EditorTimingLine>
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private readonly List<EditorTimingLine> storedLines = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        settings.SnapDivisorBindable.BindValueChanged(_ => scheduleRedraw(null), true);
        map.TimingPointAdded += scheduleRedraw;
        map.TimingPointRemoved += scheduleRedraw;
        map.TimingPointUpdated += scheduleRedraw;
    }

    private void scheduleRedraw(TimingPoint _)
    {
        Schedule(() =>
        {
            storedLines.Clear();
            Clear();
            createLines();
        });
    }

    protected override void Update()
    {
        List<EditorTimingLine> toAdd = storedLines.Where(timingLine => timingLine.IsOnScreen).ToList();

        foreach (var timingLine in toAdd)
        {
            storedLines.Remove(timingLine);
            Add(timingLine);
        }

        List<EditorTimingLine> toRemove = this.Where(timingLine => !timingLine.IsOnScreen).ToList();

        foreach (var timingLine in toRemove)
        {
            Remove(timingLine, false);
            storedLines.Add(timingLine);
        }
    }

    private void createLines()
    {
        var points = map.MapInfo.TimingPoints;

        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];

            if (point.HideLines || point.Signature == 0)
                continue;

            float target = i + 1 < points.Count ? points[i + 1].Time : clock.TrackLength;
            float increase = point.Signature * point.MsPerBeat / (4 * settings.SnapDivisor);
            float position = point.Time;

            int j = 0;

            while (position < target)
            {
                storedLines.Add(new EditorTimingLine
                {
                    Time = position,
                    Colour = getSnapColor(j % settings.SnapDivisor, j)
                });
                position += increase;
                j++;
            }
        }
    }

    private Colour4 getSnapColor(int val, int i)
    {
        switch (settings.SnapDivisor)
        {
            case 1:
                return Colour4.White;

            case 2:
                return val == 0 ? Colour4.White : Colour4.Red;

            case 4:
                return val switch
                {
                    0 or 4 => Colour4.White,
                    1 or 3 => Colour4.FromHex("#0085ff"),
                    _ => Colour4.Red
                };

            case 3:
            case 6:
            case 12:
                if (val % 3 == 0) return Colour4.Red;

                return val == 0 ? Colour4.White : Colour4.Purple;

            case 8:
            case 16:
                if (val == 0) return Colour4.White;
                if ((i - 1) % 2 == 0) return Colour4.Gold;

                return i % 4 == 0 ? Colour4.Red : Colour4.FromHex("#0085ff");

            default:
                if (val != 0) return Colour4.FromHex(i % 2 == 0 ? "#af4fb8" : "#4e94b7");

                Logger.Log($"Unknown snap value: {settings.SnapDivisor}", LoggingTarget.Runtime, LogLevel.Important);
                return Colour4.White;
        }
    }
}
