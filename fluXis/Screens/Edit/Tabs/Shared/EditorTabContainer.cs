using System;
using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Events;
using fluXis.Screens.Edit.Input;
using fluXis.Screens.Edit.Modding;
using fluXis.Screens.Edit.Tabs.Charting;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Shared;

public abstract partial class EditorTabContainer : CompositeDrawable, IKeyBindingHandler<EditorKeybinding>
{
    [Resolved]
    protected Editor Editor { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected EditorSettings Settings { get; private set; }

    [Resolved]
    protected EditorKeybindingContainer Keybindings { get; private set; }

    [Resolved]
    public EditorActionStack ActionStack { get; private set; }

    [Resolved]
    protected EditorModding Modding { get; private set; }

    protected virtual MarginPadding ContentPadding => new() { Vertical = 16 };

    private ClickableContainer sidebarClickHandler;
    private PointsSidebar sidebar;

    private double scrollAccumulation;

    protected virtual void BeforeLoad() { }
    protected virtual Container CreateContentContainer() => new() { RelativeSizeAxes = Axes.Both };
    protected abstract IEnumerable<Drawable> CreateContent();
    protected abstract Drawable CreateLeftSide();
    protected abstract PointsSidebar CreatePointsSidebar();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        BeforeLoad();

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.AutoSize),
                    new(),
                    new(GridSizeMode.AutoSize),
                },
                Content = new[]
                {
                    new[]
                    {
                        CreateLeftSide(),
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = ContentPadding,
                            Children = new Drawable[]
                            {
                                CreateContentContainer().With(c =>
                                {
                                    c.CornerRadius = 12;
                                    c.Masking = true;
                                }).WithChildren(CreateContent()),
                                sidebarClickHandler = new ClickableContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Action = () => sidebar.OnWrapperClick?.Invoke()
                                }
                            }
                        },
                        sidebar = CreatePointsSidebar()
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sidebar.Expanded.BindValueChanged(v => sidebarClickHandler.FadeTo(v.NewValue ? 1 : 0), true);
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
        int direction = scroll > 0 ? 1 : -1;

        var map = Keybindings.Keymap.Scroll;

        EditorScrollAction action = e.ControlPressed switch
        {
            true when e.ShiftPressed => map.ControlShift,
            true => map.Control,
            _ => e.ShiftPressed ? map.Shift : map.Normal
        };

        onScroll(action, direction, direction);
        return true;
    }

    private void onScroll(EditorScrollAction action, int direction, float delta)
    {
        switch (action)
        {
            case EditorScrollAction.Seek:
            {
                if (scrollAccumulation != 0 && Math.Sign(scrollAccumulation) != direction)
                    scrollAccumulation = direction * (1 - Math.Abs(scrollAccumulation));

                scrollAccumulation += delta;
                scrollAccumulation *= Settings.InvertedScroll.Value ? -1 : 1;

                while (Math.Abs(scrollAccumulation) >= 1)
                {
                    seek(scrollAccumulation < 0 ? 1 : -1);
                    scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + 1) : Math.Max(0, scrollAccumulation - 1);
                }

                break;
            }

            case EditorScrollAction.Snap:
            {
                var snaps = ChartingContainer.SNAP_DIVISORS;
                var index = Array.IndexOf(snaps, Settings.SnapDivisor);
                index += direction;

                if (index < 0)
                    index = snaps.Length - 1;
                else if (index >= snaps.Length)
                    index = 0;

                Settings.SnapDivisor = snaps[index];
                break;
            }

            case EditorScrollAction.Zoom:
            {
                Settings.Zoom += direction * .1f;
                Settings.Zoom = Math.Clamp(Settings.Zoom, .5f, 5f);
                break;
            }

            case EditorScrollAction.Rate:
                EditorClock.Rate = Math.Clamp(EditorClock.Rate + direction * .05f, .2f, 2f);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
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

            case EditorKeybinding.AddNote:
            {
                var note = new NoteEvent { Time = EditorClock.CurrentTime };
                ActionStack.Add(new EventPlaceAction(note));
                sidebar.ShowPoint(note);
                return true;
            }
        }

        return false;
    }

    public virtual void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }
}
