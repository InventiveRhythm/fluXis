using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Charting;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Lines;

public abstract partial class EditorTimingLines<T> : Container<T>
    where T : EditorTimingLines<T>.Line
{
    [Resolved]
    public EditorSettings Settings { get; private set; }

    [Resolved]
    public EditorClock EditorClock { get; private set; }

    [Resolved]
    private EditorMap map { get; set; }

    private List<T> pastLines { get; } = new();
    private List<T> futureLines { get; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        Settings.SnapDivisorBindable.BindValueChanged(_ => scheduleRedraw(null), true);
        map.TimingPointAdded += scheduleRedraw;
        map.TimingPointRemoved += scheduleRedraw;
        map.TimingPointUpdated += scheduleRedraw;
    }

    private void scheduleRedraw(TimingPoint _) => Schedule(() =>
    {
        pastLines.Clear();
        futureLines.Clear();
        ClearInternal();
        createLines();
    });

    protected override void Update()
    {
        while (pastLines.Count > 0 && !pastLines[^1].BelowScreen)
        {
            var line = pastLines[^1];
            addAndSort(line);
            pastLines.Remove(line);
        }

        while (futureLines.Count > 0 && !futureLines[0].AboveScreen)
        {
            var line = futureLines[0];
            addAndSort(line);
            futureLines.Remove(line);
        }

        while (Children.Count > 0 && Children[0].BelowScreen)
        {
            var line = Children[0];
            pastLines.Add(line);
            Remove(line, false);
        }

        while (Children.Count > 0 && Children[^1].AboveScreen)
        {
            var line = Children[^1];
            futureLines.Insert(0, line);
            Remove(line, false);
        }
    }

    private void addAndSort(T line)
    {
        Add(line);
        SortInternal();
    }

    protected override int Compare(Drawable x, Drawable y)
    {
        var lineX = (T)x;
        var lineY = (T)y;

        return lineX.CompareTo(lineY);
    }

    private void createLines()
    {
        var points = map.MapInfo.TimingPoints;

        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];

            if (point.Signature == 0)
                continue;

            var target = i + 1 < points.Count ? points[i + 1].Time : EditorClock.TrackLength;
            var increase = point.MsPerBeat / Settings.SnapDivisor;

            if (increase < .1f)
                continue;

            int j = 0;

            for (var position = point.Time; position < target; position += increase)
            {
                var divisor = divisorForIndex(j, Settings.SnapDivisor);
                var line = CreateLine(position, j % (point.Signature * Settings.SnapDivisor) == 0, FluXisColors.GetEditorSnapColor(divisor));

                LoadComponent(line);

                if (line.BelowScreen)
                    pastLines.Add(line);
                else if (line.AboveScreen)
                    futureLines.Add(line);
                else
                    Add(line);

                j++;
            }
        }
    }

    private static int divisorForIndex(int index, int snap)
    {
        var beat = index % snap;
        return ChartingContainer.SNAP_DIVISORS.FirstOrDefault(divisor => beat * divisor % snap == 0);
    }

    protected abstract T CreateLine(double time, bool big, Colour4 color);
    protected abstract Vector2 GetPosition(double time);

    public abstract partial class Line : Box, IComparable<Line>
    {
        protected new double Time { get; }
        public abstract bool BelowScreen { get; }
        public abstract bool AboveScreen { get; }

        protected new EditorTimingLines<T> Parent { get; }

        protected Line(EditorTimingLines<T> parent, double time)
        {
            Parent = parent;
            Time = time;
        }

        protected override void Update() => Position = Parent.GetPosition(Time);
        public int CompareTo(Line other) => Time.CompareTo(other.Time);
    }
}
