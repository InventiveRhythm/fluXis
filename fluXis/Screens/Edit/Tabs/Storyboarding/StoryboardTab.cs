using System;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Drawables;
using fluXis.Screens.Edit.Tabs.Storyboarding.Settings;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Storyboarding;

public partial class StoryboardTab : EditorTab
{
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;
    public override string TabName => "Storyboard";
    public override EditorTabType Type => EditorTabType.Storyboard;

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private Editor editor { get; set; }

    private DependencyContainer dependencies;

    private float scrollAccumulation;
    private AspectRatioContainer aspect;
    private IdleTracker idleTracker;
    private Container loading;

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
            idleTracker = new IdleTracker(400, rebuildPreview),
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
                                new(GridSizeMode.Absolute, 700)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Theme.Background1
                                            },
                                            new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Padding = new MarginPadding(12),
                                                Child = aspect = new AspectRatioContainer(new BindableBool(true))
                                                {
                                                    CornerRadius = 12,
                                                    Masking = true
                                                }
                                            },
                                            loading = new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Alpha = 0,
                                                Children = new Drawable[]
                                                {
                                                    new Box
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Colour = Theme.Background1,
                                                        Alpha = .5f
                                                    },
                                                    new LoadingIcon
                                                    {
                                                        Anchor = Anchor.Centre,
                                                        Origin = Anchor.Centre,
                                                        Size = new Vector2(48)
                                                    }
                                                }
                                            }
                                        }
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

        rebuildPreview();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        InvokeFullyLoaded();

        map.Storyboard.ElementAdded += queueRebuild;
        map.Storyboard.ElementRemoved += queueRebuild;
        map.Storyboard.ElementUpdated += queueRebuild;

        map.ScriptChanged += queueRebuild;
    }

    private void queueRebuild(StoryboardElement _) => queueRebuild();
    private void queueRebuild(string _) => queueRebuild();

    private void queueRebuild()
    {
        loading.FadeIn(300);
        idleTracker.Reset();
    }

    private void rebuildPreview()
    {
        aspect.Clear();

        var copy = map.Storyboard.JsonCopy();

        var draw = new DrawableStoryboard(map.MapInfo, copy, editor.MapSetPath);
        LoadComponent(draw);

        aspect.AddRange(new Drawable[]
        {
            new MapBackground(map.RealmMap)
            {
                RelativeSizeAxes = Axes.Both
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = editor.BindableBackgroundDim.Value
            }
        });
        aspect.AddRange(Enum.GetValues<StoryboardLayer>().Select(x => new DrawableStoryboardLayer(clock, draw, x)).ToArray());
        ScheduleAfterChildren(() => loading.FadeOut(300));
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
