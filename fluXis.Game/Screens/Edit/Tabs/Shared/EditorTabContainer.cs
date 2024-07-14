using System;
using System.Collections.Generic;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Input;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Shared;

public abstract partial class EditorTabContainer : CompositeDrawable, IKeyBindingHandler<EditorKeybinding>
{
    [Resolved]
    protected Editor Editor { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    public EditorActionStack ActionStack { get; private set; }

    private Container content;
    private Box dim;
    private EditorToolbox toolbox;
    private ClickableContainer sidebarClickHandler;
    private PointsSidebar sidebar;

    private double scrollAccumulation;

    protected virtual void BeforeLoad() { }
    protected abstract IEnumerable<Drawable> CreateContent();
    protected abstract EditorToolbox CreateToolbox();
    protected abstract PointsSidebar CreatePointsSidebar();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        BeforeLoad();

        InternalChildren = new Drawable[]
        {
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                ChildrenEnumerable = CreateContent()
            },
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.4f),
                Alpha = 0
            },
            toolbox = CreateToolbox(),
            sidebarClickHandler = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Action = () => sidebar.OnWrapperClick?.Invoke()
            },
            sidebar = CreatePointsSidebar()
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        toolbox.Expanded.BindValueChanged(_ => Schedule(onSidebarExpand));
        sidebar.Expanded.BindValueChanged(_ => Schedule(onSidebarExpand));

        void onSidebarExpand()
        {
            var showDim = toolbox.Expanded.Value || sidebar.Expanded.Value;
            dim.FadeTo(showDim ? 1 : 0, 400, Easing.OutCubic);

            var leftSide = toolbox.Expanded.Value;
            var rightSide = sidebar.Expanded.Value;
            var bothSides = leftSide && rightSide;

            var offset = bothSides switch
            {
                false when leftSide => 40,
                false when rightSide => -40,
                _ => 0
            };

            content.MoveToX(offset, 500, Easing.OutCubic);
            sidebarClickHandler.FadeTo(rightSide ? 1 : 0);
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.Space:
            {
                if (EditorClock.IsRunning)
                    EditorClock.Stop();
                else
                    EditorClock.Start();

                return true;
            }
        }

        return false;
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        var scroll = e.ShiftPressed ? e.ScrollDelta.X : e.ScrollDelta.Y;
        int delta = scroll > 0 ? 1 : -1;

        if (scrollAccumulation != 0 && Math.Sign(scrollAccumulation) != delta)
            scrollAccumulation = delta * (1 - Math.Abs(scrollAccumulation));

        scrollAccumulation += e.ScrollDelta.Y;

        while (Math.Abs(scrollAccumulation) >= 1)
        {
            seek(scrollAccumulation < 0 ? 1 : -1);
            scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + 1) : Math.Max(0, scrollAccumulation - 1);
        }

        return true;
    }

    private void seek(int direction)
    {
        double amount = 1;

        if (EditorClock.IsRunning)
        {
            var tp = Map.MapInfo.GetTimingPoint(EditorClock.CurrentTime);
            amount *= 4 * (tp.BPM / 120);
        }

        if (direction < 1)
            EditorClock.SeekBackward(amount);
        else
            EditorClock.SeekForward(amount);
    }

    public virtual bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        switch (e.Action)
        {
            case EditorKeybinding.Undo:
                ActionStack.Undo();
                return true;

            case EditorKeybinding.Redo:
                ActionStack.Redo();
                return true;
        }

        return false;
    }

    public virtual void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }
}
