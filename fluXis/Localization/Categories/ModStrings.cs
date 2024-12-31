using fluXis.Mods;
using osu.Framework.Localisation;

namespace fluXis.Localization.Categories;

public class ModStrings : LocalizationCategory
{
    protected override string File => "mods";

    public TranslatableString GetName(IMod mod) => new(GetKey(getAcronym(mod) + "-name"), mod.Name);

    public TranslatableString GetDescription(IMod mod) => new(GetKey(getAcronym(mod) + "-description"), mod.Description);

    private static string getAcronym(IMod mod)
    {
        if (mod is RateMod)
            return "rate";

        return mod.Acronym;
    }
}
