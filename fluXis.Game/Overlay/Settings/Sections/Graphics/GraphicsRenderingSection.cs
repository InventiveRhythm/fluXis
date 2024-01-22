using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.Graphics;

public partial class GraphicsRenderingSection : SettingsSubSection
{
    public override string Title => "Rendering";
    public override IconUsage Icon => FontAwesome6.Solid.Cubes;

    [BackgroundDependencyLoader]
    private void load(GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<RendererType>
            {
                Label = "Renderer",
                Description = "Requires a restart to take effect.",
                Items = host.GetPreferredRenderersForCurrentPlatform().OrderBy(t => t).Where(t => t != RendererType.Vulkan),
                Bindable = frameworkConfig.GetBindable<RendererType>(FrameworkSetting.Renderer)
            },
            new SettingsDropdown<FrameSync>
            {
                Label = "Frame Limiter",
                Items = Enum.GetValues<FrameSync>(),
                Bindable = frameworkConfig.GetBindable<FrameSync>(FrameworkSetting.FrameSync)
            },
            new SettingsDropdown<ExecutionMode>
            {
                Label = "Threading Mode",
                Items = Enum.GetValues<ExecutionMode>(),
                Bindable = frameworkConfig.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode)
            },
            new SettingsToggle
            {
                Label = "Show FPS",
                Description = "Shows the current DrawsPerSecond and UpdatesPerSecond in the bottom right corner.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.ShowFps)
            }
        });
    }
}
