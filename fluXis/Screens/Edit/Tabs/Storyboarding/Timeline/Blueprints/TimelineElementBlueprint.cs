using System;
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
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;

public partial class TimelineElementBlueprint : SelectionBlueprint<StoryboardElement>, IHasContextMenu
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

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

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 6,
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = .2f
                }
            },
            new BlueprintHandle
            {
                DragAction = vec =>
                {
                    var newTime = timeline.TimeAtScreenSpacePosition(vec);
                    newTime = snaps.SnapTime(newTime);
                    var len = Math.Max(newTime - Object.StartTime, snaps.CurrentStep);
                    Object.EndTime = Object.StartTime + len;
                },
                StopAction = () => storyboard.Update(element)
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

    private partial class BlueprintHandle : Drawable
    {
        public Action<Vector2> DragAction { get; init; }
        public Action StopAction { get; init; }

        public BlueprintHandle()
        {
            Size = new Vector2(28, 36);
            Anchor = Origin = Anchor.CentreRight;
        }

        protected override bool OnDragStart(DragStartEvent e) => e.Button == MouseButton.Left;

        protected override void OnDrag(DragEvent e)
        {
            DragAction?.Invoke(e.ScreenSpaceMousePosition);
            base.OnDrag(e);
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            StopAction?.Invoke();
            base.OnDragEnd(e);
        }
    }
}
