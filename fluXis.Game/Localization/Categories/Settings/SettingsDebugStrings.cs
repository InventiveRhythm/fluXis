using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsDebugStrings : LocalizationCategory
{
    protected override string File => "settings-debug";

    public TranslatableString Title => Get("title", "Debug");

    public TranslatableString ShowLogOverlay => Get("show-log-overlay", "Show Log Overlay");
    public TranslatableString ImportFile => Get("import-file", "Import File");

    public TranslatableString LogAPI => Get("log-api", "Log API Responses.");
    public TranslatableString LogAPIDescription => Get("log-api-description", "Logs all API request responses to the console and log files. This might contain sensitive info like emails and tokens.");
}
