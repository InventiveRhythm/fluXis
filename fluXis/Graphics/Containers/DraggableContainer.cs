using System;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Graphics.Containers;

public partial class DraggableContainer : Container
{
    [Resolved]
    private CursorTypeContainer cursorTypeContainer { get; set; }

    private InputManager input;

    private CursorType lastCursorType => (CursorType)typeof(CursorTypeContainer)
                                                     .GetField("last", BindingFlags.NonPublic | BindingFlags.Instance)
                                                     ?.GetValue(cursorTypeContainer)!;

    public float DraggableArea = 12f;
    public Vector2 DragDelta;

    public Action<MouseDownEvent> OnMouseDownAction;
    public Action<Vector2> OnDraggingStart;
    public Action<Vector2> OnDragging;
    public Action<Vector2> OnDraggingEnd;

    /// if false then it means it would be moving if there was dragging
    public bool IsResizing;

    [Resolved]
    private GameHost host { get; set; } = null!;

    public DraggableContainer()
    {
        AutoSizeAxes = Axes.Both;
    }

    private bool isInResizeArea(Vector2 screenPos)
    {
        var anchor = getPos(screenPos);
        return anchor != 0 && anchor != Anchor.Centre;
    }

    private Anchor getPos(Vector2 screenPos)
    {
        var mouseLocal = ToLocalSpace(screenPos);

        bool nearLeft = mouseLocal.X <= (DraggableArea + Padding.Left);
        bool nearRight = mouseLocal.X >= DrawWidth - (DraggableArea + Padding.Right);
        bool nearTop = mouseLocal.Y <= (DraggableArea + Padding.Top);
        bool nearBottom = mouseLocal.Y >= DrawHeight - (DraggableArea + Padding.Bottom);

        Anchor horizontal = Anchor.x1;
        if (nearLeft) horizontal = Anchor.x0;
        else if (nearRight) horizontal = Anchor.x2;

        Anchor vertical = Anchor.y1;
        if (nearTop) vertical = Anchor.y0;
        else if (nearBottom) vertical = Anchor.y2;

        return horizontal | vertical;
    }

    private static CursorType getCursorType(Anchor anchor) => anchor switch
    {
        Anchor.TopLeft or Anchor.BottomRight => CursorType.SizeNwSe,
        Anchor.TopRight or Anchor.BottomLeft => CursorType.SizeNeSw,
        Anchor.TopCentre or Anchor.BottomCentre => CursorType.SizeVertical,
        Anchor.CentreLeft or Anchor.CentreRight => CursorType.SizeHorizontal,
        _ => CursorType.Arrow
    };

    protected override void LoadComplete()
    {
        input = GetContainingInputManager();
    }

    protected override void Update()
    {
        if (!IsHovered || IsDragged) return;

        var mousePos = input.CurrentState.Mouse.Position;

        Anchor anchor = getPos(mousePos);
        host.Window.ChangeCursor(getCursorType(anchor));

        base.Update();
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);

        host.Window.ChangeCursor(lastCursorType);
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        OnDraggingStart?.Invoke(e.ScreenSpaceMouseDownPosition);
        IsResizing = isInResizeArea(e.ScreenSpaceMouseDownPosition);
        return true;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        base.OnDragEnd(e);
        OnDraggingEnd?.Invoke(e.ScreenSpaceMousePosition);
    }

    protected override void OnDrag(DragEvent e)
    {
        DragDelta += e.Delta;
        OnDragging?.Invoke(e.ScreenSpaceMouseDownPosition);
        base.OnDrag(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        OnMouseDownAction?.Invoke(e);
        return base.OnMouseDown(e);
    }
}
