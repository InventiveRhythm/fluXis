using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Actions.Events;
using fluXis.Game.UI;
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

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;

public abstract partial class PointsList : Container
{
    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected EditorActionStack ActionStack { get; private set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private BindableList<PointListEntry> selectedEntries { get; } = new();

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
                        Height = 32,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 20,
                                Text = "Points"
                            },
                            new AddButton(CreateAddEntries())
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight
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
                            iconUp = new SpriteIcon
                            {
                                Icon = FontAwesome6.Solid.ChevronUp,
                                Size = new Vector2(16),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Alpha = 0
                            },
                            iconDown = new SpriteIcon
                            {
                                Icon = FontAwesome6.Solid.ChevronDown,
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
        var temp = selectedEntries.ToList();
        temp.ForEach(e => e.State = SelectedState.Deselected);

        ShowSettings?.Invoke(list);
    }

    private void deleteSelected()
    {
        var temp = selectedEntries.ToList();
        temp.ForEach(e => e.State = SelectedState.Deselected);

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

        ScheduleAfterChildren(() =>
        {
            objects.ForEach(o =>
            {
                var entry = flow.FirstOrDefault(e => e.Object == o);

                if (entry is null)
                    return;

                entry.State = SelectedState.Selected;
            });
        });
    }

    protected abstract void RegisterEvents();
    protected abstract PointListEntry CreateEntryFor(ITimedObject obj);
    protected abstract IEnumerable<AddButtonEntry> CreateAddEntries();

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

            entry.Selected += e =>
            {
                if (selectedEntries.Contains(e))
                    return;

                selectedEntries.Add(e);
            };

            entry.Deselected += e => selectedEntries.Remove(e);

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
        private IEnumerable<AddButtonEntry> entries { get; }

        public AddButton(IEnumerable<AddButtonEntry> entries)
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

            private Box hover;

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
                    hover = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
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
                hover.FadeTo(.2f, 50);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                hover.FadeOut(200);
            }

            protected override bool OnClick(ClickEvent e)
            {
                create();
                return false;
            }
        }
    }

    protected class AddButtonEntry
    {
        public string Text { get; }
        public Colour4 Color { get; }
        public Action CreateAction { get; }

        public AddButtonEntry(string text, Colour4 color, Action create)
        {
            Text = text;
            Color = color;
            CreateAction = create;
        }
    }

    public partial class PointsListIconButton : CircularContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        protected virtual IconUsage ButtonIcon => FontAwesome.Solid.Plus;
        protected Action Action { get; init; }

        protected Box Background { get; private set; }
        protected Box Hover { get; private set; }
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
                    Colour = FluXisColors.Background3
                },
                Hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                Icon = new SpriteIcon
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
                Hover.FadeTo(.2f, 50);
            else
                Hover.FadeOut(200);
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
