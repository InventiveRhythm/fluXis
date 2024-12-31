using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Overlay.Settings.Sections.Graphics;

public partial class GraphicsLayoutSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Layout;
    public override IconUsage Icon => FontAwesome6.Solid.WindowRestore;

    private SettingsGraphicsStrings strings => LocalizationStrings.Settings.Graphics;

    [BackgroundDependencyLoader]
    private void load(GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<WindowMode>
            {
                Label = strings.WindowMode,
                Items = host.Window.SupportedWindowModes,
                Bindable = frameworkConfig.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
            }
        });
    }
}
