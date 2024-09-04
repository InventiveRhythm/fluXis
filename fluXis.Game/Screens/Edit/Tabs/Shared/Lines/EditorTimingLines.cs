using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Lines;

public partial class EditorTimingLines : Container<EditorTimingLines.Line>
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private List<Line> pastLines { get; } = new();
    private List<Line> futureLines { get; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Masking = true;

        settings.SnapDivisorBindable.BindValueChanged(_ => scheduleRedraw(null), true);
        map.TimingPointAdded += scheduleRedraw;
        map.TimingPointRemoved += scheduleRedraw;
        map.TimingPointUpdated += scheduleRedraw;
    }

    private void scheduleRedraw(TimingPoint _)
    {
        Schedule(() =>
        {
            pastLines.Clear();
            futureLines.Clear();
            ClearInternal();
            createLines();
        });
    }

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

    private void addAndSort(Line line)
    {
        Add(line);
        SortInternal();
    }

    protected override int Compare(Drawable x, Drawable y)
    {
        var lineX = (Line)x;
        var lineY = (Line)y;

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

            var target = i + 1 < points.Count ? points[i + 1].Time : clock.TrackLength;
            var increase = point.MsPerBeat / settings.SnapDivisor;

            int j = 0;

            for (var position = point.Time; position < target; position += increase)
            {
                var divisor = divisorForIndex(j, settings.SnapDivisor);

                var line = new Line
                {
                    Time = position,
                    Colour = FluXisColors.GetEditorSnapColor(divisor)
                };

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

        foreach (int divisor in ChartingContainer.SNAP_DIVISORS)
        {
            if (beat * divisor % snap == 0)
                return divisor;
        }

        return 0;
    }

    public partial class Line : Box, IComparable<Line>
    {
        [Resolved]
        private EditorClock clock { get; set; }

        [Resolved]
        private EditorSettings settings { get; set; }

        public new double Time { get; set; }

        public bool BelowScreen => clock.CurrentTime >= Time + 1000;
        public bool AboveScreen => clock.CurrentTime <= Time - 3000 / settings.Zoom;

        public bool IsVisible => !BelowScreen && !AboveScreen;

        public Line()
        {
            RelativeSizeAxes = Axes.X;
            Height = 3;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
        }

        protected override void Update()
        {
            Y = (float)(-EditorHitObjectContainer.HITPOSITION - .5f * ((Time - clock.CurrentTime) * settings.Zoom));
        }

        public int CompareTo(Line other) => Time.CompareTo(other.Time);
    }
}
