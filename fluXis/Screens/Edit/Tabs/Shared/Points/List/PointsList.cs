using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Events;
using fluXis.Screens.Edit.Tabs.Design.Points;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.List;

public abstract partial class PointsList : Container
{
    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected EditorActionStack ActionStack { get; private set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private BindableList<PointListEntry> selectedEntries { get; } = new();
    private PointListEntry lastSelected = null;

    public Action<IEnumerable<Drawable>> ShowSettings { get; set; }
    public Action RequestClose { get; set; }

    private bool initialLoad = true;
    private FluXisScrollContainer scroll;
    private FillFlowContainer<PointListEntry> flow;

    private Bindable<PointListEntry> currentEvent = new();

    private SpriteIcon iconUp;
    private bool iconUpShown;
    private SpriteIcon iconDown;
    private bool iconDownShown;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Padding = new MarginPadding(20);

        var entries = CreateDropdownEntries().ToList();
        var bind = new Bindable<DropdownEntry>();

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new Dimension[]
            {
                new(GridSizeMode.AutoSize),
                new(GridSizeMode.Absolute, 10),
                new()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new ForcedHeightText()
                            {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                WebFontSize = 20,
                                Text = "Points",
                                Height = 32
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Spacing = new Vector2(8),
                                Children = new Drawable[]
                                {
                                    new DesignPointListDropdown
                                    {
                                        Current = bind,
                                        Items = new DropdownEntry[]
                                        {
                                            new("All", Theme.Text, () => { }, _ => true)
                                        }.Concat(entries)
                                    },
                                    new AddButton(entries),
                                    new PointsListIconButton(() =>
                                    {
                                        var i = currentEvent.Value;
                                        if (i is not null) scroll.ScrollIntoView(i);
                                    }) { ButtonIcon = FontAwesome6.Solid.MagnifyingGlass }
                                }
                            }
                        }
                    }
                },
                new[] { Empty() },
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Bottom = 10 },
                        Children = new Drawable[]
                        {
                            scroll = new FluXisScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarVisible = false,
                                Child = flow = new FillFlowContainer<PointListEntry>
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(8)
                                }
                            },
                            iconUp = new FluXisSpriteIcon
                            {
                                Icon = FontAwesome6.Solid.AngleUp,
                                Size = new Vector2(16),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Alpha = 0
                            },
                            iconDown = new FluXisSpriteIcon
                            {
                                Icon = FontAwesome6.Solid.AngleDown,
                                Size = new Vector2(16),
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.BottomCentre,
                                Alpha = 0
                            }
                        }
                    }
                }
            }
        };

        bind.ValueChanged += e =>
        {
            var func = e.NewValue.MatchFunc;

            foreach (var entry in flow)
            {
                if (func(entry.Object))
                    entry.Show();
                else
                    entry.Hide();
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        RegisterEvents();

        initialLoad = false;
        sortPoints();
    }

    private void openSettings(IEnumerable<Drawable> list)
    {
        deselectAll();
        ShowSettings?.Invoke(list);
    }

    private void deleteSelected()
    {
        var temp = selectedEntries.ToList();
        temp.ForEach(e => e.State = SelectedState.Deselected);
        lastSelected = null;

        switch (temp.Count)
        {
            case 0:
                return;

            case 1:
                ActionStack.Add(new EventRemoveAction(temp.FirstOrDefault()!.Object));
                break;

            default:
                ActionStack.Add(new EventBulkRemoveAction(temp.Select(x => x.Object)));
                break;
        }
    }

    private void duplicateSelected()
    {
        var temp = selectedEntries.ToList();
        var objects = temp.Select(e => e.CreateClone()).ToList();

        temp.ForEach(e => e.State = SelectedState.Deselected);
        lastSelected = null;

        var lowestTime = objects.Min(o => o.Time);
        objects.ForEach(o =>
        {
            o.Time -= lowestTime;
            o.Time += clock.CurrentTime;
        });

        if (temp.Count == 1)
            ActionStack.Add(new EventPlaceAction(objects.FirstOrDefault()));
        else
            ActionStack.Add(new EventBulkCloneAction(objects));

        ScheduleAfterChildren(() => objects.ForEach(o =>
        {
            var entry = flow.FirstOrDefault(e => e.Object == o);

            if (entry is null)
                return;

            entry.State = SelectedState.Selected;
        }));
    }

    protected abstract void RegisterEvents();
    protected abstract PointListEntry CreateEntryFor(ITimedObject obj);
    protected abstract IEnumerable<DropdownEntry> CreateDropdownEntries();

    protected void Create(ITimedObject obj, bool overrideTime = true, bool openSettings = true)
    {
        if (overrideTime)
            obj.Time = clock.CurrentTime;

        ActionStack.Add(new EventPlaceAction(obj));

        if (openSettings)
            flow.FirstOrDefault(e => e.Object == obj)?.OpenSettings();
    }

    private void sortPoints()
    {
        flow.OrderBy(e => e.Object.Time).ForEach(e => flow.SetLayoutPosition(e, (float)e.Object.Time));
    }

    private void selectRange(PointListEntry obj)
    {
        var timeSorted = flow.OrderBy(e => e.Object.Time).ToList();

        int p1 = timeSorted.IndexOf(lastSelected);
        int p2 = timeSorted.IndexOf(obj);
        if (p1 > p2) (p1, p2) = (p2, p1);

        for (int i = p1; i <= p2; i++)
        {
            var e = timeSorted[i];

            if (e.Alpha > 0)
            {
                e.State = SelectedState.Selected;
                select(e);
            }
        }
    }

    private void select(PointListEntry obj)
    {
        if (selectedEntries.Contains(obj))
            return;

        if (obj.State != SelectedState.Selected) obj.State = SelectedState.Selected;
        selectedEntries.Add(obj);
        lastSelected = obj;
    }

    private void deselect(PointListEntry obj)
    {
        if (lastSelected == obj) lastSelected = null;
        selectedEntries.Remove(obj);
    }

    private void deselectAll()
    {
        var temp = selectedEntries.ToList();
        temp.ForEach(e => e.State = SelectedState.Deselected);
        lastSelected = null;
    }

    protected void AddPoint(ITimedObject obj)
    {
        var entry = CreateEntryFor(obj);

        if (entry != null)
        {
            entry.CurrentEvent = currentEvent;
            entry.ShowSettings = openSettings;
            entry.RequestClose = RequestClose;
            entry.CloneSelected = duplicateSelected;
            entry.DeleteSelected = deleteSelected;
            entry.OnClone = o => Create(o);

            entry.Selected += select;
            entry.Deselected += deselect;
            entry.SelectedRange += e =>
            {
                if (lastSelected != null)
                    selectRange(e);
                else
                    select(e);
            };

            flow.Add(entry);
        }

        if (!initialLoad)
            sortPoints();
    }

    public void ShowPoint(ITimedObject obj)
    {
        var entry = flow.FirstOrDefault(e => e.Object == obj);
        entry?.OpenSettings();
    }

    protected void UpdatePoint(ITimedObject obj)
    {
        var entry = flow.FirstOrDefault(e => e.Object == obj);
        entry?.UpdateValues();

        sortPoints();
    }

    protected void RemovePoint(ITimedObject obj)
    {
        var entry = flow.FirstOrDefault(e => e.Object == obj);

        if (entry != null)
            flow.Remove(entry, true);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        var belowTime = flow.Where(e => e.Object.Time <= clock.CurrentTime);
        var min = belowTime.MaxBy(x => x.Object.Time);
        currentEvent.Value = min;

        if (!scroll.IsScrolledToStart() && !iconUpShown)
        {
            iconUpShown = true;
            iconUp.FadeIn(300);
        }
        else if (scroll.IsScrolledToStart() && iconUpShown)
        {
            iconUpShown = false;
            iconUp.FadeOut(300);
        }

        if (!scroll.IsScrolledToEnd() && !iconDownShown)
        {
            iconDownShown = true;
            iconDown.FadeIn(300);
        }
        else if (scroll.IsScrolledToEnd() && iconDownShown)
        {
            iconDownShown = false;
            iconDown.FadeOut(300);
        }
    }

    protected override bool OnClick(ClickEvent e) => true;

    private partial class AddButton : PointsListIconButton, IHasPopover
    {
        private IEnumerable<DropdownEntry> entries { get; }

        public AddButton(IEnumerable<DropdownEntry> entries)
            : base(null)
        {
            this.entries = entries;
            Action = this.ShowPopover;
        }

        private void createAndHide(Action action)
        {
            action();
            this.HidePopover();
        }

        public Popover GetPopover()
        {
            return new FluXisPopover
            {
                HandleHover = false,
                ContentPadding = 0,
                BodyRadius = 5,
                // makes sense that BOTTOM LEFT is the
                // right one to display it from the TOP RIGHT
                AllowableAnchors = new[] { Anchor.BottomLeft },
                Padding = new MarginPadding(20),
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    ChildrenEnumerable = entries.Select(x => new Entry(x.Text, x.Color, () => createAndHide(x.CreateAction)))
                }
            };
        }

        private partial class Entry : CompositeDrawable
        {
            private string text { get; }
            private Action create { get; }

            private HoverLayer hover;

            public Entry(string text, Colour4 color, Action create)
            {
                this.text = text;
                this.create = create;

                Colour = color;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Size = new Vector2(240, 44);

                InternalChildren = new Drawable[]
                {
                    hover = new HoverLayer(),
                    new FluXisSpriteText
                    {
                        Margin = new MarginPadding(12),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 16,
                        Text = text
                    }
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                hover.Show();
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                hover.Hide();
            }

            protected override bool OnClick(ClickEvent e)
            {
                create();
                return false;
            }
        }
    }

    public class DropdownEntry
    {
        public string Text { get; }
        public Colour4 Color { get; }
        public Action CreateAction { get; }
        public Func<ITimedObject, bool> MatchFunc { get; }

        public DropdownEntry(string text, Colour4 color, Action create, Func<ITimedObject, bool> matchFunc)
        {
            Text = text;
            Color = color;
            CreateAction = create;
            MatchFunc = matchFunc;
        }
    }

    public partial class PointsListIconButton : CircularContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        public IconUsage ButtonIcon { get; init; } = FontAwesome.Solid.Plus;
        protected Action Action { get; init; }

        protected Box Background { get; private set; }
        protected HoverLayer Hover { get; private set; }
        protected SpriteIcon Icon { get; private set; }

        public PointsListIconButton(Action action)
        {
            Action = action;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(32);
            Masking = true;

            InternalChildren = new Drawable[]
            {
                Background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background3
                },
                Hover = new HoverLayer(),
                Icon = new FluXisSpriteIcon
                {
                    Icon = ButtonIcon,
                    Size = new Vector2(16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected virtual void UpdateColors(bool hovered)
        {
            if (hovered)
                Hover.Show();
            else
                Hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            Action?.Invoke();
            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            UpdateColors(true);
            return false; // else the list closes
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            UpdateColors(false);
        }
    }
}
