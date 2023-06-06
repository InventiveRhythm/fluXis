using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class DebugSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Bug;
    public override string Title => "Debug";

    [BackgroundDependencyLoader]
    private void load(FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = "Show Log Overlay",
                Bindable = frameworkConfig.GetBindable<bool>(FrameworkSetting.ShowLogOverlay)
            }
        });
    }
}
