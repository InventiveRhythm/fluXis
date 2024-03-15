using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.BottomBar.Timeline;

public partial class TimelineDensity : FillFlowContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private Track track => clock.Track.Value;

    private const int sections = 200;
    private const float section_width = 1f / sections;

    private float sectionLength => (float)(track.Length / sections);

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Size = new Vector2(1, 6);
        Anchor = Origin = Anchor.Centre;
        CornerRadius = 3;
        Masking = true;
        Y = 8;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        clock.TrackChanged += trackChanged;
        map.HitObjectAdded += _ => updateDensity();
        map.HitObjectRemoved += _ => updateDensity();

        trackChanged(track);
    }

    private void trackChanged(Track track)
    {
        Clear();

        for (var i = 0; i < sections; i++)
        {
            Add(new Box
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
        var counts = new int[sections];

        for (int i = 0; i < sections; i++)
        {
            var start = sectionLength * i;
            var end = start + sectionLength;

            var density = map.MapInfo.HitObjects.Count(h => h.Time >= start && h.EndTime <= end);
            counts[i] = density;
        }

        var highest = counts.Max();

        var pecentages = counts.Select(c => (float)c / highest).ToArray();

        for (var i = 0; i < sections; i++)
        {
            var box = this[i];
            box.Alpha = pecentages[i];
        }
    }

    protected override bool OnHover(HoverEvent e)
    {
        this.ResizeHeightTo(12, 300, Easing.OutQuint);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.ResizeHeightTo(6, 600, Easing.OutQuint);
    }
}
