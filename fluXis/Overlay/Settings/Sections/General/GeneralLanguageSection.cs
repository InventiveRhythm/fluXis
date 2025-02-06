using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.General;

public partial class GeneralLanguageSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Language;
    public override IconUsage Icon => FontAwesome6.Solid.EarthAmericas;

    private SettingsGeneralStrings strings => LocalizationStrings.Settings.General;

    [BackgroundDependencyLoader]
    private void load(FluXisGameBase game, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<Language>
            {
                Label = strings.LanguageCurrent,
                Items = game.SupportedLanguages,
                Bindable = game.CurrentLanguage
            },
            new SettingsToggle
            {
                Label = "Prefer original metadata",
                Description = "Displays song metadata in its original language.",
                Bindable = frameworkConfig.GetBindable<bool>(FrameworkSetting.ShowUnicode)
            }
        });
    }
}
