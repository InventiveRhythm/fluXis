using System;
using fluXis.Configuration;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Graphics.Containers;

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

    public float ScrollbarMargin
    {
        get => Scrollbar.Margin.Left + Scrollbar.Margin.Right;
        set => Scrollbar.Margin = new MarginPadding { Horizontal = value };
    }

    public Anchor ScrollbarOrigin
    {
        get => Scrollbar.Origin;
        set => Scrollbar.Origin = value;
    }

    private bool isDragging;
    private bool shouldDrag(MouseButtonEvent e) => e.Button == MouseButton.Middle && AllowDragScrolling;

    private Bindable<bool> relativeMiddleScroll;
    private Vector2 dragStart;
    private Vector2 dragCurrent;

    public FluXisScrollContainer(Direction direction = Direction.Vertical)
        : base(direction)
    {
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        relativeMiddleScroll = config.GetBindable<bool>(FluXisSetting.RelativeMiddleScroll);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Scrollbar.Anchor = ScrollbarAnchor;
        Scrollbar.Origin = ScrollbarOrigin;
    }

    public void ScrollTo(T entry)
    {
        if (entry == null) return;
        if (!entry.IsPresent) return;

        var pos = GetChildPosInContent(entry);
        ScrollTo(Math.Min(pos, ScrollableExtent));
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (!shouldDrag(e)) return false;

        mouseScroll(e);
        isDragging = true;
        dragStart = dragCurrent = e.MousePosition;
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (shouldDrag(e)) isDragging = false;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (!isDragging) return false;

        dragCurrent = e.MousePosition;
        mouseScroll(e);
        return true;
    }

    private void mouseScroll(UIEvent e)
    {
        if (relativeMiddleScroll.Value)
            return;

        float contentSize = Content.DrawSize[ScrollDim] - DrawSize[ScrollDim] + Padding.Top + Padding.Bottom;
        ScrollTo(ToLocalSpace(e.ScreenSpaceMousePosition)[ScrollDim] / DrawSize[ScrollDim] * contentSize, true, 0.02);
    }

    protected override void Update()
    {
        base.Update();

        if (!isDragging || !relativeMiddleScroll.Value)
            return;

        var dist = dragCurrent - dragStart;
        var amount = (ScrollDirection == Direction.Vertical ? dist.Y : dist.X) * (Time.Elapsed / 100);
        ScrollTo(Math.Clamp(Current + amount, -64, ScrollableExtent + 64), false);
    }

    protected override bool OnKeyDown(KeyDownEvent e) => false;

    protected override ScrollbarContainer CreateScrollbar(Direction direction) => new FluXisScrollBar(direction);

    protected partial class FluXisScrollBar : ScrollbarContainer
    {
        private Circle circle { get; }

        public FluXisScrollBar(Direction direction)
            : base(direction)
        {
            Child = circle = new Circle
            {
                RelativeSizeAxes = direction == Direction.Vertical ? Axes.Y : Axes.X,
                Width = direction == Direction.Vertical ? 8 : 1,
                Height = direction == Direction.Vertical ? 1 : 8,
                Colour = Theme.Background4,
                Alpha = .6f
            };
        }

        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
            Vector2 size = new Vector2(10)
            {
                [(int)ScrollDirection] = val
            };
            this.ResizeTo(size, duration, easing);
        }

        protected override void Update()
        {
            base.Update();

            circle.Anchor = Anchor;
            circle.Origin = Origin;
        }
    }
}
