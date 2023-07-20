using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Graphics.Scroll;

public partial class FluXisScrollContainer : FluXisScrollContainer<Drawable>
{
    public FluXisScrollContainer(Direction direction = Direction.Vertical)
        : base(direction)
    {
    }
}

public partial class FluXisScrollContainer<T> : BasicScrollContainer<T> where T : Drawable
{
    protected override bool IsDragging => base.IsDragging || isDragging;

    public bool AllowDragScrolling = true;

    private bool isDragging;
    private bool shouldDrag(MouseButtonEvent e) => e.Button == MouseButton.Middle && AllowDragScrolling;

    public FluXisScrollContainer(Direction direction = Direction.Vertical)
        : base(direction)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Scrollbar.Anchor = ScrollbarAnchor;
        Scrollbar.Origin = ScrollbarAnchor;
    }

    public void ScrollTo(T entry)
    {
        if (entry == null) return;
        if (!entry.IsPresent) return;

        var pos1 = GetChildPosInContent(entry);
        var pos2 = GetChildPosInContent(entry, entry.DrawSize);

        var min = Math.Min(pos1, pos2);
        var max = Math.Max(pos1, pos2);

        if (min < Current || (min > Current && entry.DrawSize[ScrollDim] > DisplayableContent))
            ScrollTo(min);
        else if (max > Current + DisplayableContent)
            ScrollTo(max - DisplayableContent);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (!shouldDrag(e)) return false;

        mouseScroll(e);
        isDragging = true;
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (shouldDrag(e)) isDragging = false;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (!isDragging) return false;

        mouseScroll(e);
        return true;
    }

    private void mouseScroll(UIEvent e)
    {
        float contentSize = Content.DrawSize[ScrollDim] - DrawSize[ScrollDim] + Padding.Top + Padding.Bottom;
        ScrollTo(ToLocalSpace(e.ScreenSpaceMousePosition)[ScrollDim] / DrawSize[ScrollDim] * contentSize, true, 0.02);
    }

    protected override bool OnKeyDown(KeyDownEvent e) => false;

    protected override ScrollbarContainer CreateScrollbar(Direction direction) => new FluXisScrollBar(direction);

    protected partial class FluXisScrollBar : ScrollbarContainer
    {
        public FluXisScrollBar(Direction direction)
            : base(direction)
        {
            CornerRadius = 4;
            Masking = true;
            Margin = new MarginPadding(2) { Left = -10 };

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Surface2
            };
        }

        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
            Vector2 size = new Vector2(8)
            {
                [(int)ScrollDirection] = val
            };
            this.ResizeTo(size, duration, easing);
        }
    }
}
