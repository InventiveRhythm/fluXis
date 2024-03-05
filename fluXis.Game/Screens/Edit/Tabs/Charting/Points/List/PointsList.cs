using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.List.Entries;
using osu.Framework.Allocation;
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

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.List;

public partial class PointsList : Container
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public Action<IEnumerable<Drawable>> ShowSettings { get; init; }
    public Action RequestClose { get; init; }

    private bool initialLoad = true;
    private FillFlowContainer<PointListEntry> flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            ScrollbarVisible = false,
            Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Padding = new MarginPadding(20),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 30,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 20,
                                Text = "Points"
                            },
                            new AddButton(create)
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight
                            }
                        }
                    },
                    flow = new FillFlowContainer<PointListEntry>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(10)
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        values.MapInfo.TimingPointAdded += addPoint;
        values.MapInfo.TimingPointChanged += updatePoint;
        values.MapInfo.TimingPointRemoved += removePoint;
        values.MapInfo.TimingPoints.ForEach(addPoint);

        values.MapInfo.ScrollVelocityAdded += addPoint;
        values.MapInfo.ScrollVelocityChanged += updatePoint;
        values.MapInfo.ScrollVelocityRemoved += removePoint;
        values.MapInfo.ScrollVelocities.ForEach(addPoint);

        values.MapEvents.LaneSwitchEventAdded += addPoint;
        values.MapEvents.LaneSwitchEventChanged += updatePoint;
        values.MapEvents.LaneSwitchEventRemoved += removePoint;
        values.MapEvents.LaneSwitchEvents.ForEach(addPoint);

        initialLoad = false;
        sortPoints();
    }

    private void create(TimedObject obj)
    {
        obj.Time = (float)clock.CurrentTime;
        values.MapInfo.Add(obj);

        var entry = flow.FirstOrDefault(e => e.Object == obj);
        entry?.OpenSettings();
    }

    private void sortPoints()
    {
        flow.OrderBy(e => e.Object.Time).ForEach(e => flow.SetLayoutPosition(e, e.Object.Time));
    }

    private void addPoint(TimedObject obj)
    {
        PointListEntry entry = obj switch
        {
            TimingPoint timing => new TimingPointEntry(timing),
            ScrollVelocity scroll => new ScrollVelocityEntry(scroll),
            LaneSwitchEvent lane => new LaneSwitchEntry(lane),
            _ => null
        };

        if (entry != null)
        {
            entry.ShowSettings = ShowSettings;
            entry.RequestClose = RequestClose;
            flow.Add(entry);
        }

        if (!initialLoad)
            sortPoints();
    }

    private void updatePoint(TimedObject obj)
    {
        var entry = flow.FirstOrDefault(e => e.Object == obj);
        entry?.UpdateValues();

        sortPoints();
    }

    private void removePoint(TimedObject obj)
    {
        var entry = flow.FirstOrDefault(e => e.Object == obj);

        if (entry != null)
            flow.Remove(entry, true);
    }

    private partial class AddButton : PointsListIconButton, IHasPopover
    {
        private Action<TimedObject> create { get; }

        public AddButton(Action<TimedObject> create)
            : base(null)
        {
            this.create = create;
            Action = this.ShowPopover;
        }

        private void createAndHide(TimedObject obj)
        {
            create(obj);
            this.HidePopover();
        }

        public Popover GetPopover()
        {
            return new FluXisPopover
            {
                HandleHover = false,
                ContentPadding = 0,
                BodyRadius = 5,
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new Entry("Timing Point", () => createAndHide(new TimingPoint { BPM = 120 })),
                        new Entry("Scroll Velocity", () => createAndHide(new ScrollVelocity { Multiplier = 1 }))
                    }
                }
            };
        }

        private partial class Entry : CompositeDrawable
        {
            private string text { get; }
            private Action create { get; }

            public Entry(string text, Action create)
            {
                this.text = text;
                this.create = create;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Size = new Vector2(200, 30);

                InternalChildren = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 16,
                        Text = text,
                        Margin = new MarginPadding(10)
                    }
                };
            }

            protected override bool OnClick(ClickEvent e)
            {
                create();
                return false;
            }
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
            Size = new Vector2(30);
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
