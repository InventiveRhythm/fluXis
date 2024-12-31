using System;
using fluXis.Graphics.Sprites;
using fluXis.Screens.Edit.Tabs.Storyboarding.Settings;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Storyboarding;

public partial class StoryboardTab : EditorTab
{
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;
    public override string TabName => "Storyboard";

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private DependencyContainer dependencies;

    private float scrollAccumulation;

    [BackgroundDependencyLoader]
    private void load()
    {
        var timeline = new StoryboardTimeline();

        dependencies.CacheAs(map.Storyboard);
        dependencies.CacheAs(timeline);
        dependencies.CacheAs<ITimePositionProvider>(timeline);
        dependencies.CacheAs(timeline.Blueprints);

        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new Dimension[]
                {
                    new(),
                    new(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new Dimension[]
                            {
                                new(),
                                new(GridSizeMode.Absolute, 560)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both
                                    },
                                    new StoryboardElementSettings()
                                }
                            }
                        }
                    },
                    new Drawable[]
                    {
                        timeline
                    }
                }
            }
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.Space:
                if (clock.IsRunning)
                    clock.Stop();
                else
                    clock.Start();

                return true;
        }

        return false;
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        // this will be handled by the timeline to scroll up/down
        if (e.ShiftPressed || e.ControlPressed)
            return false;

        var scroll = e.ScrollDelta.Y;
        int delta = scroll > 0 ? 1 : -1;

        if (scrollAccumulation != 0 && Math.Sign(scrollAccumulation) != delta)
            scrollAccumulation = delta * (1 - Math.Abs(scrollAccumulation));

        scrollAccumulation += scroll;

        while (Math.Abs(scrollAccumulation) >= 1)
        {
            seek(scrollAccumulation < 0 ? 1 : -1);
            scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + 1) : Math.Max(0, scrollAccumulation - 1);
        }

        return true;
    }

    private void seek(int direction)
    {
        double amount = 1;

        if (clock.IsRunning)
        {
            var tp = map.MapInfo.GetTimingPoint(clock.CurrentTime);
            amount *= 4 * (tp.BPM / 120);
        }

        if (direction < 1)
            clock.SeekBackward(amount);
        else
            clock.SeekForward(amount);
    }
}
