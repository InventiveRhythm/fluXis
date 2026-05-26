using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Charting;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Edit.UI.Variable.Timing;

/// <summary>
/// This Drawable is very expensive, it's recommended to only instantiate it once and use <see cref="EditorVariableWaveform.CreateView(Drawable)"/>.
/// </summary>
public partial class EditorVariableWaveform : BufferedContainer
{
    private const int count = 7;

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [CanBeNull]
    private TimingPoint point { get; set; }

    private readonly List<Drawable> trackedDrawables = [];

    private double currentTime = -10;
    private double endTime;

    private int lastIndex = -1;
    private double lastUpdateTime;

    // update every half beat since we already only move by a beat (caps at 30 fps or 1800 bpm)
    private double updateInterval => Math.Max((point?.MsPerBeat ?? 500) / 2, 1000d / 30);

    private Sample metronomeSample;
    private Sample metronomeEndSample;

    public EditorVariableWaveform(TimingPoint point = null)
        : base(cachedFrameBuffer: true)
    {
        if (point is not null) ChangePoint(point);
    }

    public BufferedContainerView<Drawable> CreateView(Drawable trackedDrawable)
    {
        trackedDrawables.Add(trackedDrawable);
        return base.CreateView();
    }

    public void ChangePoint(TimingPoint point)
    {
        this.point = point;
        currentTime = -10;
        lastIndex = -1;

        if (IsLoaded) recalculateRange();
        ForceRedraw();
    }

    public void Untrack(Drawable drawableToUntrack)
    {
        trackedDrawables.Remove(drawableToUntrack);
    }

    private void updateBuffer(double _)
    {
        if (Clock.CurrentTime - lastUpdateTime < updateInterval) return;
        if (!trackedDrawables.Any(d => d.IsPresent)) return;

        ForceRedraw();
        lastUpdateTime = Clock.CurrentTime;
    }

    private void recalculateRange()
    {
        var next = map.MapInfo.TimingPoints
                      .SkipWhile(g => g != point).Skip(1)
                      .FirstOrDefault();

        endTime = next?.Time ?? clock.TrackLength;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        metronomeSample = samples.Get("UI/metronome");
        metronomeEndSample = samples.Get("UI/metronome-end");

        clock.TimeChanged += updateBuffer; // only ever draw when time actually changes

        RelativeSizeAxes = Axes.X;
        Height = 280;
        Width = 0.4f;
        CornerRadius = 20;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Theme.Background3
        };

        for (int i = 0; i < count; i++)
            Add(new WaveformRow(i, i == count / 2));

        Add(new Box
        {
            Width = 4,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        recalculateRange();
    }

    protected override void Update()
    {
        base.Update();

        if (point == null) return;

        int currentBeat = (int)Math.Floor((clock.CurrentTime - point.Time) / point.MsPerBeat);
        goToBeat(currentBeat);
    }

    private void goToBeat(int beat)
    {
        if (point != null) goToTime(point.Time + beat * point.MsPerBeat);
    }

    private void goToTime(double time)
    {
        if (currentTime == time) return;

        currentTime = time;
        regenerate();
    }

    private void regenerate()
    {
        if (point == null) return;

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

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
            clock.TimeChanged -= updateBuffer;
        base.Dispose(isDisposing);
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
                    Colour = Theme.Background4,
                    Alpha = middle ? 1 : 0
                },
                graph = new WaveformGraph
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Waveform = editor.Waveform.Value,
                    Resolution = 0.75f,
                    BaseColour = Theme.Highlight.Darken(1f),
                    LowColour = Theme.Highlight.Darken(.5f),
                    MidColour = Theme.Highlight,
                    HighColour = Theme.Highlight.Lighten(.5f)
                }
            };
        }

        public void WaveformOffsetTo(double value) =>
            this.TransformTo(nameof(graphPosition), (float)value);
    }
}
