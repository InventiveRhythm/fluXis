using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class ModSelectStrings : LocalizationCategory
{
    protected override string File => "modselect";

    public TranslatableString Title => new(GetKey("title"), "Gameplay Modifiers");
    public TranslatableString Description => new(GetKey("description"), "Make the game harder or easier for yourself!");

    public TranslatableString RateSection => new(GetKey("section-rate"), "Rate");
    public TranslatableString DifficultyIncreaseSection => new(GetKey("section-difficulty-increase"), "Difficulty Increase");
    public TranslatableString DifficultyDecreaseSection => new(GetKey("section-difficulty-decrease"), "Difficulty Decrease");
    public TranslatableString MiscSection => new(GetKey("section-misc"), "Miscellaneous");
    public TranslatableString AutomationSection => new(GetKey("section-automation"), "Automation");

    public TranslatableString MaxScore(int percent) => new(GetKey("max-score"), "Max Score: {0:0}%", percent);
    public TranslatableString IncompatibleMods => new(GetKey("incompatible-mods"), "Incompatible Mods:");
}
