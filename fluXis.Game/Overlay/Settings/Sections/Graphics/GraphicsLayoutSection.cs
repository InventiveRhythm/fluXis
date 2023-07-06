using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.Graphics;

public partial class GraphicsLayoutSection : SettingsSubSection
{
    public override string Title => "Layout";

    [BackgroundDependencyLoader]
    private void load(GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<WindowMode>
            {
                Label = "Window Mode",
                Items = host.Window.SupportedWindowModes,
                Bindable = frameworkConfig.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
            }
        });
    }
}
