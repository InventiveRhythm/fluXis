using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Lines;

public partial class EditorTimingLines : Container<EditorTimingLine>
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private EditorMapInfo mapInfo => values.MapInfo;

    private readonly List<EditorTimingLine> storedLines = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        createLines();

        changeHandler.SnapDivisorChanged += scheduleRedraw;
        changeHandler.OnTimingPointAdded += scheduleRedraw;
        changeHandler.OnTimingPointRemoved += scheduleRedraw;
        changeHandler.OnTimingPointChanged += scheduleRedraw;
    }

    private void scheduleRedraw()
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
        for (int i = 0; i < mapInfo.TimingPoints.Count; i++)
        {
            var point = mapInfo.TimingPoints[i];

            if (point.HideLines || point.Signature == 0)
                continue;

            float target = i + 1 < mapInfo.TimingPoints.Count ? mapInfo.TimingPoints[i + 1].Time : clock.TrackLength;
            float increase = point.Signature * point.MsPerBeat / (4 * values.SnapDivisor);
            float position = point.Time;

            int j = 0;

            while (position < target)
            {
                storedLines.Add(new EditorTimingLine
                {
                    Time = position,
                    Colour = getSnapColor(j % values.SnapDivisor, j)
                });
                position += increase;
                j++;
            }
        }
    }

    private Colour4 getSnapColor(int val, int i)
    {
        switch (values.SnapDivisor)
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

                Logger.Log($"Unknown snap value: {values.SnapDivisor}", LoggingTarget.Runtime, LogLevel.Important);
                return Colour4.White;
        }
    }
}
