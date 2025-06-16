using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;

public partial class TimelineElementBlueprint : SelectionBlueprint<StoryboardElement>, IHasContextMenu
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    [Resolved]
    private Storyboard storyboard { get; set; }

    public MenuItem[] ContextMenuItems => new List<MenuItem>
    {
        new MenuActionItem("Clone", FontAwesome6.Solid.Clone, MenuItemType.Normal, clone),
        new MenuActionItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, delete),
    }.ToArray();

    public override double FirstComparer => Object.StartTime;
    public override double SecondComparer => Object.EndTime;

    public override Vector2 ScreenSpaceSelectionPoint => Drawable.ScreenSpaceDrawQuad.TopLeft;

    public TimelineElementBlueprint(StoryboardElement element)
        : base(element)
    {
        Height = 36;
        Anchor = Origin = Anchor.TopLeft;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 6,
            Masking = true,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = .2f
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Parent == null)
            return;

        var start = Parent.ToLocalSpace(timeline.ScreenSpacePositionAtTime(Object.StartTime, Object.ZIndex));
        var end = Parent.ToLocalSpace(timeline.ScreenSpacePositionAtTime(Object.EndTime, Object.ZIndex));

        Position = start;
        Width = end.X - start.X;
    }

    private void clone()
    {
        timeline.CloneElement(Object);
    }

    private void delete()
    {
        storyboard.Remove(Object);
    }
}
