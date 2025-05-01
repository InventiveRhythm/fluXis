using System;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Settings.Sidebar;

public partial class SettingsSidebar : Container
{
    public Action<Drawable> ScrollToSection { get; init; }

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
                Padding = new MarginPadding(12),
                Spacing = new Vector2(4)
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

            var first = true;

            foreach (var subSection in section.SubSections)
            {
                flow.Add(new SettingsSidebarButton(subSection)
                {
                    Margin = new MarginPadding { Top = first ? 0 : 4 },
                    ClickAction = () => ScrollToSection?.Invoke(subSection)
                });

                foreach (var sub in subSection.OfType<SettingsSubSectionTitle>())
                {
                    flow.Add(new SettingsSidebarSubButton(sub)
                    {
                        ClickAction = () => ScrollToSection?.Invoke(sub)
                    });
                }

                first = false;
            }
        }, true);
    }
}
