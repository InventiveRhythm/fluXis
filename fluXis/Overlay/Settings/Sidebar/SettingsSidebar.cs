using System;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Settings.Sidebar;

public partial class SettingsSidebar : Container
{
    public Action<SettingsSubSection> ScrollToSection { get; init; }

    private Bindable<SettingsSection> currentSection { get; }

    private FillFlowContainer flow;

    public SettingsSidebar(Bindable<SettingsSection> bind)
    {
        currentSection = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(10),
                Spacing = new Vector2(0, 10)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        currentSection.BindValueChanged(e =>
        {
            var section = e.NewValue;

            flow.Clear();
            flow.AddRange(section.SubSections.Select(s => new SettingsSidebarButton(s)
            {
                ClickAction = () => ScrollToSection?.Invoke(s)
            }));
        }, true);
    }
}
