using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GraphicsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Desktop;
    public override string Title => "Graphics";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config, GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new SettingsItem[]
        {
            new SettingsDropdown<RendererType>
            {
                Label = "Renderer (requires restart)",
                Items = host.GetPreferredRenderersForCurrentPlatform().OrderBy(t => t).Where(t => t != RendererType.Vulkan),
                Bindable = frameworkConfig.GetBindable<RendererType>(FrameworkSetting.Renderer)
            },
            new SettingsDropdown<WindowMode>
            {
                Label = "Window Mode",
                Items = host.Window.SupportedWindowModes,
                Bindable = frameworkConfig.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
            },
            new SettingsDropdown<FrameSync>
            {
                Label = "Frame Limiter",
                Items = Enum.GetValues<FrameSync>(),
                Bindable = frameworkConfig.GetBindable<FrameSync>(FrameworkSetting.FrameSync)
            },
            new SettingsSlider<float>
            {
                Label = "Background Dim",
                Bindable = config.GetBindable<float>(FluXisSetting.BackgroundDim),
                DisplayAsPercentage = true
            },
            new SettingsSlider<float>
            {
                Label = "Background Blur",
                Bindable = config.GetBindable<float>(FluXisSetting.BackgroundBlur),
                DisplayAsPercentage = true
            },
            new SettingsToggle
            {
                Label = "Background Pulse",
                Bindable = config.GetBindable<bool>(FluXisSetting.BackgroundPulse)
            }
        });
    }
}
