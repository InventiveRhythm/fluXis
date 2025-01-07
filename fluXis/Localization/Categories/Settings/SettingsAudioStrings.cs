using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsAudioStrings : LocalizationCategory
{
    protected override string File => "settings-audio";

    public TranslatableString Title => Get("title", "Audio");

    #region Device

    public TranslatableString Device => Get("device-title", "Device");

    public TranslatableString OutputDevice => Get("output-device", "Output Device");

    #endregion

    #region Volume

    public TranslatableString Volume => Get("volume-title", "Volume");

    public TranslatableString MasterVolume => Get("master-volume", "Master Volume");
    public TranslatableString MasterVolumeInactive => Get("master-volume-inactive", "Master Volume (Inactive)");
    public TranslatableString MasterVolumeInactiveDescription => Get("master-volume-inactive-description", "Volume when the game is inactive (multiplied by Master Volume)");
    public TranslatableString MusicVolume => Get("music-volume", "Music Volume");
    public TranslatableString EffectVolume => Get("effect-volume", "Effect Volume");
    public TranslatableString HitSoundVolume => Get("hit-sound-volume", "Hit Sound Volume");

    #endregion

    #region Offset

    public TranslatableString Offset => Get("offset-title", "Offset");

    public TranslatableString AudioOffset => Get("audio-offset", "Audio Offset");
    public TranslatableString AudioOffsetDescription => Get("audio-offset-description", "Changes the audio offset for all songs. Higher means objects appear later.");

    public TranslatableString DisableOffsetInReplay => Get("disable-offset-in-replay", "Disable Offset in Replays");
    public TranslatableString DisableOffsetInReplayDescription => Get("disable-offset-in-replay-description", "Disables the audio offset when viewing replays. This is useful for recording videos of replays.");

    public TranslatableString OpenOffsetWizard => Get("open-offset-wizard", "Open Offset wizard");

    #endregion

    #region Panning

    public TranslatableString Panning => Get("panning-title", "Panning");

    public TranslatableString UIPanning => Get("ui-panning", "User Interface");
    public TranslatableString UIPanningDescription => Get("ui-panning-description", "Changes the panning for click and hover sounds based on the mouse position.");

    public TranslatableString HitsoundPanning => Get("hitsound-panning", "Hitsounds");
    public TranslatableString HitsoundPanningDescription => Get("hitsound-panning-description", "Changes the panning for hitsounds based on the playfield position.");

    #endregion
}
