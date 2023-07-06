using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class EditorTimeline : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    private CircularContainer currentTimeIndicator;
    private Container timingPoints;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding { Left = 10, Right = 10 };

        Children = new Drawable[]
        {
            new CircularContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = 5,
                Masking = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            },
            currentTimeIndicator = new CircularContainer
            {
                Width = 3,
                Height = 30,
                Masking = true,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                RelativePositionAxes = Axes.X,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            },
            timingPoints = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Y = -7
            }
        };

        foreach (var timingPoint in values.Editor.MapInfo.TimingPoints)
        {
            timingPoints.Add(new CircularContainer
            {
                Width = 5,
                Height = 5,
                Masking = true,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                RelativePositionAxes = Axes.X,
                X = timingPoint.Time == 0 ? 0 : timingPoint.Time / clock.TrackLength,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            });
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition);
        return true;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition);
        return true;
    }

    protected override void OnDrag(DragEvent e) => seekToMousePosition(e.MousePosition);

    private void seekToMousePosition(Vector2 position)
    {
        float x = Math.Clamp(position.X, 0, DrawWidth);
        float progress = x / DrawWidth;
        float time = progress * clock.TrackLength;
        clock.SeekSmoothly(time);
    }

    protected override void Update()
    {
        currentTimeIndicator.X = (float)(clock.CurrentTime / clock.TrackLength);
    }
}
