using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsDebugStrings : LocalizationCategory
{
    protected override string File => "settings-debug";

    public TranslatableString Title => Get("title", "Debug");

    public TranslatableString ShowLogOverlay => Get("show-log-overlay", "Show Log Overlay");
    public TranslatableString ImportFile => Get("import-file", "Import File");
    public TranslatableString InstallUpdateFromFile => Get("update-from-file", "Install Update From File");

    public TranslatableString InstallUpdateFromFileDescription =>
        Get("update-from-file-description", "Installs an update from a .zip file. Be careful from where you download the file from though!");
}
