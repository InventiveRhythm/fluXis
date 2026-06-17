using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;
using fluXis.Storyboards;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK.Input;
using Vector2 = osuTK.Vector2;

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
        new MenuActionItem("Clone", Phosphor.Bold.Copy, MenuItemType.Normal, clone),
        new MenuActionItem("Delete", Phosphor.Bold.Trash, MenuItemType.Dangerous, delete),
    }.ToArray();

    public override double FirstComparer => Object.StartTime;
    public override double SecondComparer => Object.EndTime;

    public override RectangleF ScreenSpaceSelectionRect
    {
        get
        {
            var ss = Drawable.ScreenSpaceDrawQuad;
            return new RectangleF(ss.TopLeft, ss.Size);
        }
    }

    [CanBeNull]
    public Action<SelectionBlueprint<StoryboardElement>, Vector2, bool> OnHandleDrag;

    [CanBeNull]
    public Action OnHandleDragFinished;

    private readonly BlueprintHandle leftHandle;
    private readonly BlueprintHandle rightHandle;

    public TimelineElementBlueprint(StoryboardElement element)
        : base(element)
    {
        Height = TimelineElement.HEIGHT;
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
            leftHandle = new BlueprintHandle(this)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                DragAction = vec => OnHandleDrag?.Invoke(this, vec, false),
                StopAction = () => OnHandleDragFinished?.Invoke()
            },
            rightHandle = new BlueprintHandle(this)
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                DragAction = vec => OnHandleDrag?.Invoke(this, vec, true),
                StopAction = () => OnHandleDragFinished?.Invoke()
            }
        };
    }

    protected override bool ShouldBeConsideredForInput(Drawable child)
    {
        if (child == leftHandle || child == rightHandle)
            return true;

        return false;
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

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        OnHandleDrag = null;
        OnHandleDragFinished = null;
    }

    private partial class BlueprintHandle : Drawable, IHasCursorType
    {
        CursorType IHasCursorType.Cursor => parent.IsSelected ? CursorType.SizeHorizontal : CursorType.Ignore;

        private readonly TimelineElementBlueprint parent;

        public Action<Vector2> DragAction { get; init; }
        public Action StopAction { get; init; }

        public BlueprintHandle(TimelineElementBlueprint parent)
        {
            this.parent = parent;
            Size = new Vector2(28, 36);
            AlwaysPresent = true;
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
