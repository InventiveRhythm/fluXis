using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Overlay.Settings.Sections;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Settings.Tabs;

public partial class SettingsCategorySelector : Container
{
    private List<SettingsSection> sections { get; }
    private Bindable<SettingsSection> currentSection { get; }

    public SettingsCategoryTab ExperimentsTab { get; private set; }
    public Action CloseAction { get; init; }

    public SettingsCategorySelector(List<SettingsSection> list, Bindable<SettingsSection> bind)
    {
        sections = list;
        currentSection = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 90;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background3
            },
            new FillFlowContainer<SettingsCategoryTab>
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding(10),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                ChildrenEnumerable = sections.Select(createTab)
            },
            new IconButton
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Icon = FontAwesome6.Solid.XMark,
                ButtonSize = 70,
                Action = CloseAction,
                Margin = new MarginPadding(10)
            }
        };
    }

    private SettingsCategoryTab createTab(SettingsSection s)
    {
        var tab = new SettingsCategoryTab(s, currentSection);

        if (s is ExperimentsSection)
        {
            tab.Alpha = 0;
            ExperimentsTab = tab;
        }

        return tab;
    }
}
