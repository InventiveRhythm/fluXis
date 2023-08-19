using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class TimingCategoryList<T> : Container
    where T : TimingCategoryList<T>.ListEntry
{
    [Resolved]
    public EditorChangeHandler ChangeHandler { get; private set; }

    [Resolved]
    public EditorValues EditorValues { get; private set; }

    [Resolved]
    public EditorClock EditorClock { get; private set; }

    private readonly FillFlowContainer<T> flow;
    private readonly TimingTab timingTab;

    protected TimingCategoryList(string title, Colour4 background, TimingTab tab)
    {
        timingTab = tab;
        RelativeSizeAxes = Axes.Both;

        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = background
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Bottom = 80, Top = 10 },
                Masking = true,
                Child = new FluXisScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarAnchor = Anchor.TopRight,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = title,
                            FontSize = 32,
                            Margin = new MarginPadding { Bottom = 10, Left = 10 }
                        },
                        flow = new FillFlowContainer<T>
                        {
                            Padding = new MarginPadding { Top = 48 },
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 70,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Padding = new MarginPadding { Bottom = 10, Horizontal = 10 },
                Child = new AddButton { Action = OnAdd }
            }
        });
    }

    public void AddEntry(T item)
    {
        item.TimingTab = timingTab;
        item.List = this;
        flow.Add(item);
    }

    public IReadOnlyList<T> GetEntries()
    {
        return flow.Children;
    }

    public void ReplaceEntries(IEnumerable<T> entries)
    {
        flow.Clear(false);
        flow.AddRange(entries);
    }

    public virtual void OnAdd()
    {
        Sort();
    }

    public virtual void Sort() { }

    public virtual void Remove(T entry) => flow.Remove(entry, true);

    private partial class AddButton : CircularContainer
    {
        public Action Action;

        private readonly Box hoverBox;

        public AddButton()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Masking = true;

            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background4
                },
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(20),
                    Child = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10, 0),
                        Children = new Drawable[]
                        {
                            new SpriteIcon
                            {
                                Icon = FontAwesome.Solid.Plus,
                                Size = new Vector2(20),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new FluXisSpriteText
                            {
                                Text = "Add",
                                FontSize = 24,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            });
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (e.Button == MouseButton.Left)
                Action?.Invoke();

            hoverBox.FadeTo(0.4f)
                    .FadeTo(.2f, 400);

            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.FadeTo(0.2f, 200);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hoverBox.FadeTo(0, 200);
            base.OnHoverLost(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(0.95f, 4000, Easing.OutQuint);
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1, 800, Easing.OutElastic);
            base.OnMouseUp(e);
        }
    }

    public partial class ListEntry : Container, IHasContextMenu
    {
        public TimingTab TimingTab;
        public TimingCategoryList<T> List;

        public ListEntry()
        {
            RelativeSizeAxes = Axes.X;
            Height = 30;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White,
                    Alpha = 0
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Margin = new MarginPadding { Left = 10 },
                    Spacing = new Vector2(10, 0),
                    Children = CreateContent()
                }
            });
        }

        protected override bool OnClick(ClickEvent e)
        {
            TimingTab.SetPointSettings(CreatePointSettings());
            return true;
        }

        protected virtual void Remove() { }

        public virtual Drawable[] CreateContent() => Array.Empty<Drawable>();

        public virtual PointSettings CreatePointSettings() => null;

        public MenuItem[] ContextMenuItems => new MenuItem[]
        {
            new FluXisMenuItem("Remove", MenuItemType.Dangerous, Remove)
        };
    }
}
