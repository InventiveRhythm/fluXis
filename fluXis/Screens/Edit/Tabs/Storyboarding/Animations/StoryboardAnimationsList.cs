using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Storyboarding.Animations.Blueprints;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Lines;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations;

public partial class StoryboardAnimationsList : CompositeDrawable, ITimePositionProvider
{
    public const float ROW_HEIGHT = 24;
    public const float SIDE_WIDTH = 140;

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private TimelineBlueprintContainer timelineBlueprints { get; set; }
    public StoryboardAnimationBlueprintContainer Blueprints { get; set; } = new();

    private readonly StoryboardTimeline timeline;
    private FillFlowContainer<StoryboardAnimationRow> rowsFlow;
    public IEnumerable<StoryboardAnimationEntry> AnimationsEnumerable => 
        rowsFlow.Children.SelectMany(r => r.GetEntries());
    private Box dim;
    private FluXisSpriteText text;

    public event Action<StoryboardAnimationEntry> AnimationAdded;
    public event Action<StoryboardAnimationEntry> AnimationRemoved;
    public event Action<StoryboardAnimationEntry> AnimationUpdated;
    public event Action<bool> FocusedElement;

    public StoryboardAnimationsList(StoryboardTimeline timeline)
    {
        this.timeline = timeline;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new Box
            {
                Width = SIDE_WIDTH,
                RelativeSizeAxes = Axes.Y,
                Colour = Theme.Background1
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = SIDE_WIDTH },
                Child = new Box
                {
                    Width = 4,
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .5f,
                    Colour = Theme.Text
                }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = ROW_HEIGHT,
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, SIDE_WIDTH),
                    new()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = "Parameters",
                            WebFontSize = 12,
                            Margin = new MarginPadding { Horizontal = 8 },
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        new StoryboardTimingLines(timeline)
                        {
                            RelativeSizeAxes = Axes.Both,
                            LineAnchor = Anchor.TopLeft,
                            LineOrigin = Anchor.TopCentre
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    rowsFlow = new()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = ROW_HEIGHT },
                        Direction = FillDirection.Vertical
                    },
                    Blueprints
                }
            },
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background1.Opacity(0.75f),
                Alpha = 0
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        timelineBlueprints.SelectionHandler.SelectedObjects.BindCollectionChanged(collectionChanged, true);

        map.RegisterAddListener<StoryboardAnimation>(TriggerAnimationAdded);
        map.RegisterUpdateListener<StoryboardAnimation>(TriggerAnimationUpdated);
    }

    public void CloneAnimation(StoryboardAnimation anim, StoryboardAnimationRow row, float offset = 0)
    {
        var copy = anim.JsonCopy();

        copy.StartTime = clock.CurrentTime + offset;

        row.Add(copy);
    }

    public StoryboardAnimationEntry GetDrawable(StoryboardAnimation anim)
        => AnimationsEnumerable.FirstOrDefault(e => e.Animation == anim);

    public StoryboardAnimationRow GetAnimationRow(StoryboardAnimation anim)
        => rowsFlow.Children.FirstOrDefault(row => row.GetEntries().Any(e => e.Animation == anim));

    public int GetRowIndex(StoryboardAnimationRow row)
        => rowsFlow.IndexOf(row);

    public float PositionAtTime(double time, float w)
    {
        var timelineWidth = w - SIDE_WIDTH;
        var timelineCenter = SIDE_WIDTH + timelineWidth / 2;
        return (float)(timelineCenter + .5f * ((time - clock.CurrentTime) * settings.Zoom));
    }

    public float PositionAtTime(double time) => PositionAtTime(time, DrawWidth);

    public Vector2 ScreenSpacePositionAtTime(double time, int rowIdx)
        => ToScreenSpace(new Vector2(PositionAtTime(time), (rowIdx * ROW_HEIGHT) + ROW_HEIGHT));

    public double TimeAtPosition(float x)
    {
        var timelineWidth = DrawWidth - SIDE_WIDTH;
        var timelineCenter = SIDE_WIDTH + timelineWidth / 2;
        return (x - timelineCenter) * 2 / settings.Zoom + clock.CurrentTime;
    }

    public double TimeAtScreenSpacePosition(Vector2 pos) => TimeAtPosition(ToLocalSpace(pos).X);

    public void TriggerAnimationAdded(StoryboardAnimation anim) => AnimationAdded?.Invoke(GetDrawable(anim));
    public void TriggerAnimationRemoved(StoryboardAnimation anim) => AnimationRemoved?.Invoke(GetDrawable(anim));
    public void TriggerAnimationUpdated(StoryboardAnimation anim) => AnimationUpdated?.Invoke(GetDrawable(anim));

    private void collectionChanged(object _, NotifyCollectionChangedEventArgs e)
    {
        var collection = timelineBlueprints.SelectionHandler.SelectedObjects;
        
        rowsFlow.Clear();

        text.Text = "";
        dim.FadeOut(200);

        switch (collection.Count)
        {
            case 0:
                text.Text = "Nothing selected.";
                dim.FadeIn(200);
                FocusedElement?.Invoke(false);
                break;

            case 1:
                var entries = Enum.GetValues<StoryboardAnimationType>();
                var item = collection.First();

                var attrs = new List<AllowedAnimationAttribute>
                {
                    new(StoryboardAnimationType.MoveX),
                    new(StoryboardAnimationType.MoveY),
                    new(StoryboardAnimationType.Scale),
                    new(StoryboardAnimationType.ScaleVector),
                    new(StoryboardAnimationType.Rotate),
                    new(StoryboardAnimationType.Fade),
                    new(StoryboardAnimationType.Color)
                };

                if (item.Type.TryGetAllAttributes<StoryboardElementType, AllowedAnimationAttribute>(out var ext))
                    attrs.AddRange(ext);

                var rem = attrs.Where(x => x.IsDeny).ToList();
                rem.ForEach(x => attrs.RemoveAll(y => y.Type == x.Type));

                rowsFlow.ChildrenEnumerable = attrs.Select(x => new StoryboardAnimationRow(item, x.Type));
                FocusedElement?.Invoke(true);
                break;

            default:
                text.Text = "Too many selections.";
                dim.FadeIn(200);
                FocusedElement?.Invoke(false);
                break;
        }
    }
}
