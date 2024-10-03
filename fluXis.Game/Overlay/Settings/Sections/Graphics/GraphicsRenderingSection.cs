using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Panel.Types;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    private Bindable<RendererType> rendererBindable;

    [BackgroundDependencyLoader]
    private void load(GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<RendererType>
            {
                Label = strings.Renderer,
                Description = strings.RendererDescription,
                Items = host.GetPreferredRenderersForCurrentPlatform(),
                Bindable = rendererBindable = frameworkConfig.GetBindable<RendererType>(FrameworkSetting.Renderer)
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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        rendererBindable.BindValueChanged(rendererChanged);
    }

    private void rendererChanged(ValueChangedEvent<RendererType> renderer)
    {
        if (panels == null)
            return;

        var panel = new ButtonPanel
        {
            Text = "Restart Required",
            SubText = "Changing the renderer requires a restart to take effect.",
            Icon = FontAwesome6.Solid.ArrowsRotate,
            Buttons = new ButtonData[]
            {
                new PrimaryButtonData("Restart now.", () => game.Exit(true)),
                new CancelButtonData("I'll do it later.")
            }
        };

        panels.Content = panel;
    }
}
