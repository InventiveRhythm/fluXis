using System;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline;

public partial class EditorTimeline : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private TimelineIndicator indicator;

    private const double debounce = 50;

    private double? lastSeekTime;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            new Circle
            {
                Colour = Theme.Text,
                RelativeSizeAxes = Axes.X,
                Height = 5,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new TimelineTagContainer() { Offset = 10 },
            new TimelineDensity(),
            indicator = new TimelineIndicator()
        };
    }

    protected override void Update()
    {
        base.Update();

        if (!IsDragged)
        {
            var x = clock.CurrentTime / clock.TrackLength;
            if (!double.IsFinite(x) || double.IsNaN(x)) x = 0;

            indicator.X = (float)x;
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition, instant: true);
        return true;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition, instant: false);
        return true;
    }

    protected override void OnDrag(DragEvent e) => seekToMousePosition(e.MousePosition, instant: false);

    protected override void OnDragEnd(DragEndEvent e)
    {
        base.OnDragEnd(e);
        seekToMousePosition(e.MousePosition, instant: true);
    }

    private void seekToMousePosition(Vector2 position, bool instant)
    {
        // why is there a 20px offset??
        float x = Math.Clamp(position.X - 20, 0, DrawWidth);
        float progress = x / DrawWidth;
        double targetTime = progress * clock.TrackLength;

        var indicatorPos = targetTime / clock.TrackLength;
        if (!double.IsFinite(indicatorPos) || double.IsNaN(indicatorPos)) indicatorPos = 0;
        indicator.X = (float)indicatorPos;

        if (!instant && lastSeekTime != null && Time.Current - lastSeekTime < debounce)
            return;

        clock.SeekSmoothly(targetTime);
        lastSeekTime = instant ? null : Time.Current;
    }
}
