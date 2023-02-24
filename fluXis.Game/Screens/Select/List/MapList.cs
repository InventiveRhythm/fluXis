using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Select.List;

public partial class MapList : BasicScrollContainer
{
    private bool isDragging;
    private bool shouldDrag(MouseButtonEvent e) => e.Button == MouseButton.Right;

    protected override bool IsDragging => base.IsDragging || isDragging;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        RelativeSizeAxes = Axes.Both;
        Width = .5f;
        Masking = false;
        Padding = new MarginPadding(10) { Left = 20, Top = 105 };
    }

    protected override void Update()
    {
        for (var i = 0; i < Content.Children.Count; i++)
        {
            var child = Content.Children[i];

            int prevIndex = i - 1;

            if (i > 0)
            {
                while (Content.Children[prevIndex].Alpha == 0 && prevIndex > 0)
                    prevIndex--;
            }

            if (i > 0 && Content.Children[prevIndex].Alpha != 0)
                child.Y = Content.Children[prevIndex].Y + Content.Children[prevIndex].Height + 5;
            else
                child.Y = 0;
        }

        base.Update();
    }

    public void ScrollTo(MapListEntry entry)
    {
        var pos1 = GetChildPosInContent(entry);
        var pos2 = GetChildPosInContent(entry, entry.DrawSize);

        var min = Math.Min(pos1, pos2);
        var max = Math.Max(pos1, pos2);

        if (min < Current || (min > Current && entry.DrawSize[ScrollDim] > DisplayableContent))
            ScrollTo(min);
        else if (max > Current + DisplayableContent)
            ScrollTo(max - DisplayableContent);
    }

    private void rightMouseScroll(MouseEvent e)
    {
        float contentSize = Content.DrawSize[ScrollDim] - DrawSize[ScrollDim] + Padding.Top + Padding.Bottom;
        ScrollTo(ToLocalSpace(e.ScreenSpaceMousePosition)[ScrollDim] / DrawSize[ScrollDim] * contentSize, true, 0.02);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (shouldDrag(e))
        {
            rightMouseScroll(e);
            isDragging = true;
            return true;
        }

        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (shouldDrag(e))
        {
            isDragging = false;
            return;
        }

        base.OnMouseUp(e);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (isDragging)
        {
            rightMouseScroll(e);
            return true;
        }

        return base.OnMouseMove(e);
    }

    protected override ScrollbarContainer CreateScrollbar(Direction direction) => new MapListScrollbar(direction);

    protected partial class MapListScrollbar : ScrollbarContainer
    {
        public MapListScrollbar(Direction direction)
            : base(direction)
        {
            CornerRadius = 4;
            Masking = true;
            Margin = new MarginPadding(2) { Left = -10 };

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            };
        }

        protected override void LoadComplete()
        {
            Anchor = Origin = Anchor.TopLeft;
            base.LoadComplete();
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
