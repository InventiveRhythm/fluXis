using fluXis.Game.Mods;
using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class ModStrings : LocalizationCategory
{
    protected override string File => "mods";

    public LocalisableString GetName(IMod mod) => new TranslatableString(GetKey(mod.Acronym + "-name"), mod.Name);
    public LocalisableString GetDescription(IMod mod) => new TranslatableString(GetKey(mod.Acronym + "-description"), mod.Description);
}
