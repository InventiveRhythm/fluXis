using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsDebugStrings : LocalizationCategory
{
    protected override string File => "settings-debug";

    public TranslatableString Title => Get("title", "Debug");

    public TranslatableString ShowLogOverlay => Get("show-log-overlay", "Show Log Overlay");
    public TranslatableString ImportFile => Get("import-file", "Import File");

    public TranslatableString StreamFileBrowser => Get("stream-file-browser", "Stream Files in File Browser.");
    public TranslatableString StreamFileBrowserDescription => Get("stream-file-browser-description", "Streams files and folders for large directories. Turning this off might result in a game freeze when opening a large directory.");

    public TranslatableString LogAPI => Get("log-api", "Log API Responses.");
    public TranslatableString LogAPIDescription => Get("log-api-description", "Logs all API request responses to the console and log files. This might contain sensitive info like emails and tokens.");

    public TranslatableString ShowMissingLocalizations => Get("show-missing-localizations", "Show missing localizations.");
    public TranslatableString ShowMissingLocalizationsDescription => Get("show-missing-localizations-description", "Shows the ID of strings that do not have a localization defined. (switch languages to update)");
}
