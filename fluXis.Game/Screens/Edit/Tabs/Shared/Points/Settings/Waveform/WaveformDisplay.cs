using System;
using System.Linq;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Waveform;

public partial class WaveformDisplay : Container
{
    private const int count = 7;

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private TimingPoint point { get; }

    private double currentTime;
    private double endTime;

    private int lastIndex = -1;

    private Sample metronomeSample;
    private Sample metronomeEndSample;

    public WaveformDisplay(TimingPoint point)
    {
        this.point = point;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        metronomeSample = samples.Get("UI/metronome");
        metronomeEndSample = samples.Get("UI/metronome-end");

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

        var next = map.MapInfo.TimingPoints
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

    private void goToTime(double time)
    {
        if (currentTime == time) return;

        currentTime = time;
        regenerate();
    }

    private void regenerate()
    {
        var index = (int)Math.Round((currentTime - point.Time) / point.MsPerBeat);

        // 10ms leniency
        if (clock.IsRunning && lastIndex != index && clock.CurrentTime > point.Time - 10 && clock.CurrentTime < endTime + 10)
        {
            if (index % point.Signature == 0)
                metronomeEndSample?.Play();
            else
                metronomeSample?.Play();
        }

        lastIndex = index;

        var trackLength = clock.TrackLength;
        var scale = trackLength / DrawWidth;

        const int middle = count / 2;
        index -= middle;

        foreach (var row in InternalChildren.OfType<WaveformRow>())
        {
            var time = point.Time + index * point.MsPerBeat;
            var offset = (time - DrawWidth / 2 + ChartingContainer.WAVEFORM_OFFSET) / trackLength * scale;

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
        private Editor editor { get; set; }

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
                    Waveform = editor.Waveform.Value,
                    Resolution = 1,
                    BaseColour = FluXisColors.Highlight.Darken(1f),
                    LowColour = FluXisColors.Highlight.Darken(.5f),
                    MidColour = FluXisColors.Highlight,
                    HighColour = FluXisColors.Highlight.Lighten(.5f)
                }
            };
        }

        public void WaveformOffsetTo(double value) =>
            this.TransformTo(nameof(graphPosition), (float)value);
    }
}
