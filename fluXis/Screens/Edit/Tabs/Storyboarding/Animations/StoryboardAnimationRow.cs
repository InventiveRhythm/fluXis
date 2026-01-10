using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Storyboards;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations;

public partial class StoryboardAnimationRow : GridContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private StoryboardAnimationsList animationList { get; set; }

    private readonly StoryboardElement item;
    private readonly StoryboardAnimationType type;
    private readonly Colour4 color;

    private Container<StoryboardAnimationEntry> entries;

    public StoryboardAnimationRow(StoryboardElement item, StoryboardAnimationType type)
    {
        this.item = item;
        this.type = type;
        color = getColor(type);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = StoryboardAnimationsList.ROW_HEIGHT;
        ColumnDimensions = new Dimension[]
        {
            new(GridSizeMode.Absolute, StoryboardAnimationsList.SIDE_WIDTH),
            new()
        };

        Content = new[]
        {
            new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = $"{type.GetDescription()}",
                            WebFontSize = 12,
                            Margin = new MarginPadding { Horizontal = 8 },
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Colour = color
                        },
                        new ClickableContainer
                        {
                            Size = new Vector2(StoryboardAnimationsList.ROW_HEIGHT),
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Action = addNew,
                            Child = new FluXisSpriteIcon
                            {
                                Icon = FontAwesome6.Solid.Plus,
                                Margin = new MarginPadding { Horizontal = 8 },
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(12)
                            }
                        },
                    }
                },
                entries = new Container<StoryboardAnimationEntry>
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    ChildrenEnumerable = item.Animations.Where(x => x.Type == type).Select(createEntry)
                }
            }
        };
    }

    private void addNew()
    {
        var animation = new StoryboardAnimation
        {
            StartTime = clock.CurrentTime,
            ValueStart = getDefault(type),
            ValueEnd = getDefault(type),
            Type = type
        };

        item.Animations.Add(animation);
        entries.Add(createEntry(animation));
        map.Add(animation);
    }

    public void Add(StoryboardAnimation animation)
    {
        var copy = animation.JsonCopy();
        entries.Add(createEntry(copy));
        map.Add(copy);
    }

    public void Remove(StoryboardAnimation animation)
    {
        var entry = entries.FirstOrDefault(x => x.Animation == animation);
        if (entry != null)
        {
            animationList.TriggerAnimationRemoved(animation);
            entries.Remove(entry, true);
        }
        
        item.Animations.Remove(animation);
        map.Remove(animation);
    }

    public void UpdateAnim(StoryboardAnimation animation) => map.Update(animation);

    private StoryboardAnimationEntry createEntry(StoryboardAnimation anim)
        => new(anim, this, color) { RequestRemove = Remove };

    private static string getDefault(StoryboardAnimationType type)
    {
        switch (type)
        {
            case StoryboardAnimationType.MoveX:
            case StoryboardAnimationType.MoveY:
            case StoryboardAnimationType.Width:
            case StoryboardAnimationType.Height:
            case StoryboardAnimationType.Rotate:
            case StoryboardAnimationType.Border:
                return "0";

            case StoryboardAnimationType.Scale:
            case StoryboardAnimationType.Fade:
                return "1";

            case StoryboardAnimationType.ScaleVector:
                return "1,1";

            case StoryboardAnimationType.Color:
                return "#ffffff";

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private static Colour4 getColor(StoryboardAnimationType type) => type switch
    {
        StoryboardAnimationType.MoveX => Theme.Red,
        StoryboardAnimationType.MoveY => Theme.Orange,
        StoryboardAnimationType.Scale => Theme.Yellow,
        StoryboardAnimationType.ScaleVector => Theme.Lime,
        StoryboardAnimationType.Width => Theme.Green,
        StoryboardAnimationType.Height => Theme.Aqua,
        StoryboardAnimationType.Rotate => Theme.Cyan,
        StoryboardAnimationType.Fade => Theme.Blue,
        StoryboardAnimationType.Color => Theme.Purple,
        StoryboardAnimationType.Border => Theme.Pink,
        _ => Theme.Text
    };

    public IEnumerable<StoryboardAnimationEntry> GetEntries()
    {
        return entries.Children.AsEnumerable();
    }
}
