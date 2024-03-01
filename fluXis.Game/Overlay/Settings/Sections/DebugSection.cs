using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Files;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class DebugSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Bug;
    public override LocalisableString Title => strings.Title;

    private SettingsDebugStrings strings => LocalizationStrings.Settings.Debug;

    [BackgroundDependencyLoader]
    private void load(FrameworkConfigManager frameworkConfig, FluXisGameBase game, PanelContainer panels)
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.ShowLogOverlay,
                Bindable = frameworkConfig.GetBindable<bool>(FrameworkSetting.ShowLogOverlay)
            },
            new SettingsButton
            {
                Label = strings.ImportFile,
                ButtonText = "Import",
                Action = () =>
                {
                    panels.Content = new FileSelect
                    {
                        OnFileSelected = file => game.HandleDragDrop(file.FullName)
                    };
                }
            },
            new SettingsButton
            {
                Label = strings.InstallUpdateFromFile,
                Description = strings.InstallUpdateFromFileDescription,
                ButtonText = "Find",
                Action = () =>
                {
                    panels.Content = new FileSelect
                    {
                        AllowedExtensions = new[] { ".zip" },
                        OnFileSelected = file => game.CreateUpdatePerformer()?.UpdateFromFile(file)
                    };
                }
            }
        });
    }
}
