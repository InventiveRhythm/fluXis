using fluXis.Game.Localization;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.General;

public partial class GeneralLanguageSection : SettingsSubSection
{
    public override string Title => "Language";
    public override IconUsage Icon => FontAwesome.Solid.GlobeAmericas;

    [BackgroundDependencyLoader]
    private void load(FluXisGameBase game)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<Language>
            {
                Label = "Current Language",
                Items = game.SupportedLanguages,
                Bindable = game.CurrentLanguage
            }
        });
    }
}
