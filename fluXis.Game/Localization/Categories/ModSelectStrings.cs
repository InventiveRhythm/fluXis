using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class ModSelectStrings : LocalizationCategory
{
    protected override string File => "modselect";

    public LocalisableString Title => new TranslatableString(GetKey("title"), "Gameplay Modifiers");
    public LocalisableString Description => new TranslatableString(GetKey("description"), "Make the game harder or easier for yourself!");

    public LocalisableString RateSection => new TranslatableString(GetKey("section-rate"), "Rate");
    public LocalisableString DifficultyIncreaseSection => new TranslatableString(GetKey("section-difficulty-increase"), "Difficulty Increase");
    public LocalisableString DifficultyDecreaseSection => new TranslatableString(GetKey("section-difficulty-decrease"), "Difficulty Decrease");
    public LocalisableString MiscSection => new TranslatableString(GetKey("section-misc"), "Miscellaneous");
    public LocalisableString AutomationSection => new TranslatableString(GetKey("section-automation"), "Automation");

    public LocalisableString MaxScore(int percent) => new TranslatableString(GetKey("max-score"), "Max Score: {0:0}%", percent);
    public LocalisableString IncompatibleMods => new TranslatableString(GetKey("incompatible-mods"), "Incompatible Mods:");
}
