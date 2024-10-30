using System;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Tabs;

public partial class TabControl : CompositeDrawable
{
    public TabContainer[] Tabs { get; init; } = Array.Empty<TabContainer>();

    private Bindable<TabContainer> current;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        if (Tabs.Length <= 0)
            throw new ArgumentException($"{nameof(Tabs)} must have 1 or more children.");

        current = new Bindable<TabContainer>(Tabs.First());

        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(16),
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(24),
                    ChildrenEnumerable = Tabs.Select(t => new TabControlItem(t, current))
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = Tabs
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        current.BindValueChanged(currentChanged, true);
        FinishTransforms(true);
    }

    private void currentChanged(ValueChangedEvent<TabContainer> e)
    {
        foreach (var tab in Tabs)
        {
            if (tab != e.NewValue)
                tab.FadeOut(150);
        }

        e.NewValue.Delay(100).FadeIn(150);
    }

    private partial class TabControlItem : FillFlowContainer
    {
        private TabContainer container { get; }
        private Bindable<TabContainer> current { get; }

        public TabControlItem(TabContainer container, Bindable<TabContainer> current)
        {
            this.container = container;
            this.current = current;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(8);

            InternalChildren = new Drawable[]
            {
                new SpriteIcon
                {
                    Size = new Vector2(16),
                    Icon = container.Icon,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                new FluXisSpriteText
                {
                    Text = container.Title,
                    WebFontSize = 16,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            current.BindValueChanged(currentChanged, true);
            FinishTransforms();
        }

        private void currentChanged(ValueChangedEvent<TabContainer> e)
        {
            this.FadeTo(e.NewValue == container ? 1 : .6f, 150);
        }

        protected override bool OnClick(ClickEvent e)
        {
            current.Value = container;
            return true;
        }
    }
}
