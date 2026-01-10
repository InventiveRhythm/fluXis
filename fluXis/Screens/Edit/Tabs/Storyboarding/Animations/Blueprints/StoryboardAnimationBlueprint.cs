using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations.Blueprints;

public partial class StoryboardAnimationBlueprint : SelectionBlueprint<StoryboardAnimation>, IHasContextMenu
{
    [Resolved]
    private StoryboardAnimationsList animationList { get; set; }

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

    [Resolved]
    private Storyboard storyboard { get; set; }

    public StoryboardAnimationRow Row;

    public MenuItem[] ContextMenuItems => new List<MenuItem>
    {
        new MenuActionItem("Clone", FontAwesome6.Solid.Clone, MenuItemType.Normal, clone),
        new MenuActionItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, delete),
    }.ToArray();

    public override double FirstComparer => Object.StartTime;
    public override double SecondComparer => Object.EndTime;

    public override Vector2 ScreenSpaceSelectionPoint => Drawable.ScreenSpaceDrawQuad.TopLeft;

    public StoryboardAnimationBlueprint(StoryboardAnimation anim, StoryboardAnimationRow row)
        : base(anim)
    {
        Row = row;

        Height = 15f;
        Anchor = Origin = Anchor.CentreLeft;

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
                    var newTime = animationList.TimeAtScreenSpacePosition(vec);
                    newTime = snaps.SnapTime(newTime);
                    var len = Math.Max(newTime - Object.StartTime, snaps.CurrentStep);
                    Object.Duration = len;
                },
                StopAction = () => Row.UpdateAnim(anim)
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Parent == null)
            return;

        var rowIndex = animationList.GetRowIndex(Row);
        var startX = animationList.PositionAtTime(Object.StartTime);
        var endX = animationList.PositionAtTime(Object.StartTime + Object.Duration);

        var row_height = StoryboardAnimationsList.ROW_HEIGHT;

        Position = new Vector2(startX - 12f, (rowIndex * row_height) + row_height + 12f);
        Width = Math.Max(TimelineElement.HEIGHT, endX - startX);
    }

    private void clone()
    {
        animationList.CloneAnimation(Object, Row);
    }

    private void delete()
    {
        Row.Remove(Object);
    }

    private partial class BlueprintHandle : Drawable
    {
        public Action<Vector2> DragAction { get; init; }
        public Action StopAction { get; init; }

        public BlueprintHandle()
        {
            // Size = new Vector2(28, 36);
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