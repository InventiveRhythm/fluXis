using System;
using System.Linq;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings.Waveform;

public partial class WaveformDisplay : Container
{
    private const int count = 7;

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private TimingPoint point { get; }

    private float currentTime;
    private float endTime;

    public WaveformDisplay(TimingPoint point)
    {
        this.point = point;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 280;
        CornerRadius = 20;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Background3
        };

        for (int i = 0; i < count; i++)
            Add(new WaveformRow(i, i == count / 2));

        Add(new Box
        {
            Width = 2,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var next = values.MapInfo.TimingPoints
                         .SkipWhile(g => g != point).Skip(1)
                         .FirstOrDefault();

        endTime = next?.Time ?? clock.TrackLength;
    }

    protected override void Update()
    {
        base.Update();

        int currentBeat = (int)Math.Floor((clock.CurrentTime - point.Time) / point.MsPerBeat);
        goToBeat(currentBeat);
    }

    private void goToBeat(int beat) => goToTime(point.Time + beat * point.MsPerBeat);

    private void goToTime(float time)
    {
        if (currentTime == time) return;

        currentTime = time;
        regenerate();
    }

    private void regenerate()
    {
        var index = (currentTime - point.Time) / point.MsPerBeat;

        var trackLength = clock.TrackLength;
        var scale = trackLength / DrawWidth;

        const int middle = count / 2;
        index -= middle;

        foreach (var row in InternalChildren.OfType<WaveformRow>())
        {
            var time = point.Time + index * point.MsPerBeat;
            var offset = (time - DrawWidth / 2) / trackLength * scale;

            row.Alpha = time < point.Time || time > endTime ? 0.2f : 1;
            row.WaveformOffsetTo(-offset);
            row.WaveformScale = new Vector2(scale, 1);

            index++;
        }
    }

    private partial class WaveformRow : Container
    {
        private int index { get; }
        private bool middle { get; }

        [Resolved]
        private EditorValues values { get; set; }

        private WaveformGraph graph;

        public Vector2 WaveformScale { set => graph.Scale = value; }

        private float graphPosition
        {
            get => graph.X;
            set => graph.X = value;
        }

        public WaveformRow(int i, bool middle)
        {
            index = i;
            this.middle = middle;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            Height = 1f / count;
            Y = (float)index / count;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background4,
                    Alpha = middle ? 1 : 0
                },
                graph = new WaveformGraph
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Waveform = values.Editor.Waveform.Value,
                    Resolution = 1,
                    BaseColour = FluXisColors.Highlight.Darken(1f),
                    LowColour = FluXisColors.Highlight.Darken(.5f),
                    MidColour = FluXisColors.Highlight,
                    HighColour = FluXisColors.Highlight.Lighten(.5f)
                }
            };
        }

        public void WaveformOffsetTo(float value) =>
            this.TransformTo(nameof(graphPosition), value);
    }
}
