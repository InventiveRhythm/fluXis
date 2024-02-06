using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.Graphics;

public partial class GraphicsRenderingSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Rendering;
    public override IconUsage Icon => FontAwesome6.Solid.Cubes;

    private SettingsGraphicsStrings strings => LocalizationStrings.Settings.Graphics;

    [BackgroundDependencyLoader]
    private void load(GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<RendererType>
            {
                Label = strings.Renderer,
                Description = strings.RendererDescription,
                Items = host.GetPreferredRenderersForCurrentPlatform().OrderBy(t => t).Where(t => t != RendererType.Vulkan),
                Bindable = frameworkConfig.GetBindable<RendererType>(FrameworkSetting.Renderer)
            },
            new SettingsDropdown<FrameSync>
            {
                Label = strings.FrameLimiter,
                Items = Enum.GetValues<FrameSync>(),
                Bindable = frameworkConfig.GetBindable<FrameSync>(FrameworkSetting.FrameSync)
            },
            new SettingsDropdown<ExecutionMode>
            {
                Label = strings.ThreadingMode,
                Items = Enum.GetValues<ExecutionMode>(),
                Bindable = frameworkConfig.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode)
            },
            new SettingsToggle
            {
                Label = strings.ShowFps,
                Description = strings.ShowFpsDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.ShowFps)
            }
        });
    }
}
