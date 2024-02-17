using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsGeneralStrings : LocalizationCategory
{
    protected override string File => "settings-general";

    public TranslatableString Title => Get("title", "General");

    #region Language

    public TranslatableString Language => Get("language-title", "Language");

    public TranslatableString LanguageCurrent => Get("language-current", "Current Language");

    #endregion

    #region Updates

    public TranslatableString Updates => Get("updates-title", "Updates");

    public TranslatableString ReleaseChannel => Get("release-channel", "Release Channel");
    public TranslatableString ReleaseChannelDescription => Get("release-channel-description", "Select the release channel to receive updates from.");

    public TranslatableString UpdatesCheck => Get("check-updates", "Check for updates");
    public TranslatableString UpdatesCheckDescription => Get("check-updates-description", "Checks for updates and downloads them if available.");

    #endregion

    #region Folders

    public TranslatableString Folders => Get("folders-title", "Folders");

    public TranslatableString FoldersOpen => Get("open-folder", "Open fluXis folder");
    public TranslatableString FoldersChange => Get("change-folder", "Change folder location");

    #endregion
}
