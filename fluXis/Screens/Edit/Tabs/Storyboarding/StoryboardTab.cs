using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Screens.Edit.Tabs.Storyboarding.Animations;
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
    private Box backgroundDim;

    private Storyboard storyboard => map.Storyboard;
    private DrawableDynamicStoryboard dynamicStoryboard;
    private DrawableDynamicStoryboardLayer[] layers;

    private readonly List<StoryboardElement> scriptsToBeRebuilt = new();
    private readonly List<StoryboardElement> scriptsToBeRemoved = new();

    private bool loadingFromRanges;

    [BackgroundDependencyLoader]
    private void load()
    {
        var timeline = new StoryboardTimeline();

        dependencies.CacheAs(map.Storyboard);
        dependencies.CacheAs(timeline);
        dependencies.CacheAs<ITimePositionProvider>(timeline);
        dependencies.CacheAs(timeline.Blueprints);

        LoadComponent(dynamicStoryboard = new DrawableDynamicStoryboard
        (
            new Bindable<MapInfo>(map.MapInfo),
            new Bindable<Storyboard>(storyboard),
            editor.MapSetPath
        ));

        layers = Enum.GetValues<StoryboardLayer>()
                     .Select(x => new DrawableDynamicStoryboardLayer(clock, dynamicStoryboard, x))
                     .ToArray();

        aspect = new AspectRatioContainer(new BindableBool(true))
        {
            CornerRadius = 12,
            Masking = true
        };
        aspect.Add(new MapBackground(map.RealmMap) { RelativeSizeAxes = Axes.Both });
        aspect.Add(backgroundDim = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Black,
            Alpha = editor.BindableBackgroundDim.Value
        });
        aspect.AddRange(layers);

        InternalChildren = new Drawable[]
        {
            dynamicStoryboard,
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
                                new(GridSizeMode.Absolute, 500),
                                new(),
                                new(GridSizeMode.Absolute, 500)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new StoryboardAnimationsList(timeline),
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
                                                Child = aspect
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
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.Storyboard.ElementAdded += onElementAdded;
        map.Storyboard.ElementRemoved += onElementRemoved;
        map.Storyboard.ElementUpdated += onElementUpdated;

        map.RegisterAddListener<StoryboardAnimation>(onAnimationUpdated);
        map.RegisterUpdateListener<StoryboardAnimation>(onAnimationUpdated);
        map.RegisterRemoveListener<StoryboardAnimation>(onAnimationUpdated);

        map.ScriptChanged += queueRebuild;
    }

    protected override void Update()
    {
        base.Update();
        updateLoadingFromRanges();
    }

    /// <summary>
    /// Not to be confused with actual loading; This only updates the loading icon's visibility
    /// </summary>
    private void updateLoadingFromRanges()
    {
        float currentTime = (float)clock.CurrentTime;
        var ranges = dynamicStoryboard.LoadedRanges;

        var active = ranges.Where(r => currentTime >= r.Item1 && currentTime <= r.Item2).ToList();

        bool shouldShow = active.Count > 0 && active.Any(r => !r.Item3);

        if (shouldShow == loadingFromRanges)
            return;

        loadingFromRanges = shouldShow;

        if (shouldShow)
            loading.FadeIn(300);
        else
            loading.FadeOut(300);
    }

    #region Event Handling

    private void onElementAdded(StoryboardElement e)
    {
        if (e.Type == StoryboardElementType.Script)
            queueRebuild(e);
        else
        {
            // TODO: maybe don't iterate over all layers in the future?
            foreach (var layer in layers)
                layer.QueueAddStaticElement(e);
        }

        storyboard.Sort();
    }

    private void onElementRemoved(StoryboardElement e)
    {
        if (e.Type == StoryboardElementType.Script)
        {
            scriptsToBeRemoved.Add(e);
            loading.FadeIn(300);
            idleTracker.Reset();
        }
        else
        {
            // TODO: maybe don't iterate over all layers in the future?
            foreach (var layer in layers)
                layer.QueueRemoveStaticElement(e);
        }

        storyboard.Sort();
    }

    private void onElementUpdated(StoryboardElement e)
    {
        if (e.Type == StoryboardElementType.Script)
            queueRebuild(e);
        else
        {
            // TODO: maybe don't iterate over all layers in the future?
            foreach (var layer in layers)
                layer.UpdateStaticElement(e);
        }

        storyboard.Sort();
    }

    private void onAnimationUpdated(StoryboardAnimation a) => onElementUpdated(a.ParentElement);

    #endregion

    #region Building

    private void queueRebuild(StoryboardElement e)
    {
        scriptsToBeRebuilt.Clear();
        scriptsToBeRebuilt.Add(e);

        loading.FadeIn(300);
        idleTracker.Reset();
    }

    private void queueRebuild(string s)
    {
        scriptsToBeRebuilt.Clear();
        scriptsToBeRebuilt.AddRange(storyboard.GetScriptElements(s));

        loading.FadeIn(300);
        idleTracker.Reset();
    }

    private void rebuildPreview()
    {
        backgroundDim.Alpha = editor.BindableBackgroundDim.Value;

        if (scriptsToBeRemoved.Count > 0)
            dynamicStoryboard.RemoveElements(scriptsToBeRemoved);

        if (scriptsToBeRebuilt.Count > 0)
            dynamicStoryboard.RebuildElements(scriptsToBeRebuilt);

        scriptsToBeRemoved.Clear();
        scriptsToBeRebuilt.Clear();

        if (!loadingFromRanges)
            ScheduleAfterChildren(() => loading.FadeOut(300));
    }

    #endregion

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

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (map == null) return;

        map.Storyboard.ElementAdded -= onElementAdded;
        map.Storyboard.ElementRemoved -= onElementRemoved;
        map.Storyboard.ElementUpdated -= onElementUpdated;

        map.DeregisterAddListener<StoryboardAnimation>(onAnimationUpdated);
        map.DeregisterUpdateListener<StoryboardAnimation>(onAnimationUpdated);
        map.DeregisterRemoveListener<StoryboardAnimation>(onAnimationUpdated);

        map.ScriptChanged -= queueRebuild;
    }
}
