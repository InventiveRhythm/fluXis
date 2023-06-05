using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool shouldDrag(MouseButtonEvent e) => e.Button == MouseButton.Middle;

    protected override bool IsDragging => base.IsDragging || isDragging;
    public new FillFlowContainer<MapListEntry> Content => flow;

    private FillFlowContainer<MapListEntry> flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = false;
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(10)
        {
            Left = 20,
            Top = 60,
            Bottom = 10
        };

        Child = flow = new FillFlowContainer<MapListEntry>
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Direction = FillDirection.Vertical
        };
    }

    public void AddMap(MapListEntry entry)
    {
        Content.Add(entry);
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

    private void mouseScroll(UIEvent e)
    {
        float contentSize = Content.DrawSize[ScrollDim] - DrawSize[ScrollDim] + Padding.Top + Padding.Bottom;
        ScrollTo(ToLocalSpace(e.ScreenSpaceMousePosition)[ScrollDim] / DrawSize[ScrollDim] * contentSize, true, 0.02);
    }

    public void Insert(int index, MapListEntry entry)
    {
        List<MapListEntry> entries = Content.Children.ToList();

        entries.Insert(index, entry);
        Content.Clear(false);
        Content.AddRange(entries);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (shouldDrag(e))
        {
            mouseScroll(e);
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
            mouseScroll(e);
            return true;
        }

        return base.OnMouseMove(e);
    }

    protected override bool OnHover(HoverEvent e) => true;

    protected override void OnHoverLost(HoverLostEvent e)
    {
        var selected = Content.Children.FirstOrDefault(c => c.Selected);
        if (selected != null) ScrollTo(selected);
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
