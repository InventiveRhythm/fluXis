using System;
using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Input;
using fluXis.Overlay.Network.Sidebar;
using fluXis.Overlay.Network.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Overlay.Network;

public partial class Dashboard : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private Container content;
    private DashboardSidebar sidebar;
    private Container<DashboardTab> tabsContainer;
    private DashboardTab selectedTab;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

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
            content = new ClickableContainer
            {
                Width = 1200,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                EdgeEffect = FluXisStyles.ShadowLargeNoOffset,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new FluXisContextMenuContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = tabsContainer = new Container<DashboardTab>
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(12)
                            {
                                Top = 50 + 12,
                                Left = DashboardSidebar.SIZE_CLOSED + 12
                            }
                        }
                    },
                    sidebar = new DashboardSidebar { SelectAction = selectTab }
                }
            }
        };

        addTab(new DashboardNotificationsTab());
        addTab(new DashboardNewsTab());
        addTab(new DashboardFriendsTab());
        addTab(new DashboardClubTab());
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

    public void Show(DashboardTabType type)
    {
        var tab = tabsContainer.FirstOrDefault(t => t.Type == type);

        if (tab == null)
            throw new InvalidOperationException($"Cannot show a tab of type {type} as it is not added to the {nameof(Dashboard)}.");

        selectTab(tab);
        Show();
    }

    protected override void PopIn()
    {
        this.FadeIn(200);
        content.MoveToX(0, 400, Easing.OutQuint);
        sidebar.MoveToX(0, 400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200);
        content.MoveToX(-50, 400, Easing.OutQuint);
        sidebar.MoveToX(-25, 400, Easing.OutQuint);
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
