using System;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline;

public partial class EditorTimeline : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private TimelineIndicator indicator;
    private SeekContainer seekContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            seekContainer = new SeekContainer
            {
                RelativeSizeAxes = Axes.Both,
                HorizontalOffset = 0,
                IsPlaying = () => clock.IsRunning,
                OnSeek = onSeek,
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
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (seekContainer.IsDragged)
        {
            indicator.X = seekContainer.Progress.Value;
        }
        else
        {
            var x = clock.CurrentTime / clock.TrackLength;
            if (!double.IsFinite(x) || double.IsNaN(x)) x = 0;

            indicator.X = (float)x;
        }
    }

    private void onSeek(float progress)
    {
        double targetTime = progress * clock.TrackLength;
        clock.SeekSmoothly(targetTime);
    }
}
