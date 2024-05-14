using fluXis.Game.Mods;
using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class ModStrings : LocalizationCategory
{
    protected override string File => "mods";

    public TranslatableString GetName(IMod mod) => new(GetKey(mod.Acronym + "-name"), mod.Name);
    public TranslatableString GetDescription(IMod mod) => new(GetKey(mod.Acronym + "-description"), mod.Description);
}
