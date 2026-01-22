using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Lines;
using fluXis.Storyboards;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations;

public partial class StoryboardAnimationsList : CompositeDrawable
{
    public const float ROW_HEIGHT = 24;
    public const float SIDE_WIDTH = 140;

    [Resolved]
    private TimelineBlueprintContainer blueprints { get; set; }

    private readonly StoryboardTimeline timeline;
    private FillFlowContainer flow;
    private Box dim;
    private FluXisSpriteText text;

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
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = ROW_HEIGHT },
                Direction = FillDirection.Vertical
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

        blueprints.SelectionHandler.SelectedObjects.BindCollectionChanged(collectionChanged, true);
    }

    private void collectionChanged(object _, NotifyCollectionChangedEventArgs __)
    {
        var collection = blueprints.SelectionHandler.SelectedObjects;
        flow.Clear();
        text.Text = "";
        dim.FadeOut(200);

        switch (collection.Count)
        {
            case 0:
                text.Text = "Nothing selected.";
                dim.FadeIn(200);
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

                flow.ChildrenEnumerable = attrs.Select(x => new StoryboardAnimationRow(item, x.Type));
                break;

            default:
                text.Text = "Too many selections.";
                dim.FadeIn(200);
                break;
        }
    }
}
