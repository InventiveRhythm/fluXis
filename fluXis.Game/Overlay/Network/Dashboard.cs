using System;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Network.Sidebar;
using fluXis.Game.Overlay.Network.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Network;

public partial class Dashboard : VisibilityContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    protected override bool StartHidden => true;

    private const int rounding = 20;

    [Resolved]
    private Fluxel fluxel { get; set; }

    private Container content;
    private DashboardSidebar sidebar;
    private Container<DashboardTab> tabsContainer;
    private DashboardTab selectedTab;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Hide,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 50, Bottom = 50, Top = 20 },
                Child = content = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = rounding,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Scale = new Vector2(.9f),
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Colour4.Black.Opacity(.25f),
                        Radius = 20,
                        Offset = new Vector2(0, 1)
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = 30 },
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ColumnDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.AutoSize),
                                    new()
                                },
                                RowDimensions = new Dimension[]
                                {
                                    new()
                                },
                                Content = new[]
                                {
                                    new Drawable[]
                                    {
                                        sidebar = new DashboardSidebar
                                        {
                                            SelectAction = selectTab
                                        },
                                        tabsContainer = new Container<DashboardTab>
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Padding = new MarginPadding(10)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        addTab(new DashboardNotificationsTab());
        addTab(new DashboardNewsTab());
        addTab(new DashboardFriendsTab());
        addTab(new DashboardOnlineTab());
        addTab(new DashboardAccountTab());
    }

    private void addTab(DashboardTab tab)
    {
        sidebar.AddTab(tab);
        tabsContainer.Add(tab);
        tab.Alpha = 0;

        if (tabsContainer.Count == 1)
            selectTab(tab);
    }

    private void selectTab(DashboardTab tab)
    {
        if (tab == null || tab == selectedTab)
            return;

        if (!tabsContainer.Contains(tab))
            throw new InvalidOperationException($"Cannot select a tab that is not added to the {nameof(Dashboard)}.");

        var i = tabsContainer.IndexOf(tab);

        foreach (var drawable in tabsContainer)
        {
            var j = tabsContainer.IndexOf(drawable);

            const int distance = 40;
            const int duration = 400;

            var button = sidebar.GetButton(drawable);

            if (button != null)
                button.Selected = drawable == tab;

            if (j < i)
                drawable.MoveToY(-distance, duration, Easing.OutQuint);
            else if (j > i)
                drawable.MoveToY(distance, duration, Easing.OutQuint);
            else
                drawable.MoveToY(0, duration, Easing.OutQuint);

            if (j == i)
                drawable.FadeIn(200);
            else
                drawable.FadeOut(200);
        }

        selectedTab?.Exit();
        selectedTab = tab;
        selectedTab.Enter();
    }

    protected override void PopIn()
    {
        this.FadeIn(200);
        content.ScaleTo(1f, 400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                Hide();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
