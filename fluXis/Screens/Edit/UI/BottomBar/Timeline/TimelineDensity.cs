using System.Linq;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline;

public partial class TimelineDensity : BufferedContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private DrawableTrack track => clock.Track.Value;

    private const int sections = 200;
    private const float section_width = 1f / sections;

    private float sectionLength => (float)(track.Length / sections);

    private FillFlowContainer flow;

    public TimelineDensity()
        : base(cachedFrameBuffer: true)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Size = new Vector2(1, 6);
        Anchor = Origin = Anchor.Centre;
        CornerRadius = 3;
        Masking = true;
        Y = 8;

        Child = flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        clock.TrackChanged += trackChanged;
        map.RegisterAddListener<HitObject>(_ => updateDensity());
        map.RegisterUpdateListener<HitObject>(_ => updateDensity());
        map.RegisterRemoveListener<HitObject>(_ => updateDensity());

        trackChanged(track);
    }

    private void trackChanged(DrawableTrack newTrack)
    {
        flow.Clear();

        for (var i = 0; i < sections; i++)
        {
            flow.Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Width = section_width,
                Colour = Colour4.FromHex("FDD27F"),
                AlwaysPresent = true,
                Alpha = 0
            });
        }

        updateDensity();
    }

    private void updateDensity()
    {
        var counts = new float[sections];

        for (int i = 0; i < sections; i++)
        {
            var start = sectionLength * i;
            var end = start + sectionLength;

            var objects = map.MapInfo.HitObjects.Where(h => h.Time >= start && h.Time < end);
            counts[i] = objects.Sum(getValue);
        }

        var highest = counts.Max();

        if (highest == 0)
            return;

        var percentages = counts.Select(c => c / highest).ToArray();

        for (var i = 0; i < sections; i++)
            flow[i].Alpha = percentages[i];

        ForceRedraw();
    }

    private float getValue(HitObject hit) => hit.Type switch
    {
        HitObjectType.Tick => .1f,
        HitObjectType.Landmine => 0f,
        _ => 1f
    };

    protected override bool OnHover(HoverEvent e)
    {
        this.ResizeHeightTo(12, 300, Easing.OutQuint).During(ForceRedraw);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.ResizeHeightTo(6, 600, Easing.OutQuint).During(ForceRedraw);
    }
}
