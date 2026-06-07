using osu.Framework.Localisation;

namespace fluXis.Localization.Categories;

public class GeneralStrings : LocalizationCategory
{
    protected override string File => "general";

    public TranslatableString General => Get("general", "General");
    public TranslatableString Back => Get("back", "Back");
    public TranslatableString Play => Get("play", "Play");
    public TranslatableString Edit => Get("edit", "Edit");
    public TranslatableString Select => Get("select", "Select");
    public TranslatableString Delete => Get("delete", "Delete");
    public TranslatableString Export => Get("export", "Export");
    public TranslatableString Refresh => Get("refresh", "Refresh");
    public TranslatableString ViewOnline => Get("view-online", "View Online");

    public TranslatableString PanelGenericConfirm => Get("panel-generic-confirm", "Yes, do it.");
    public TranslatableString PanelGenericCancel => Get("panel-generic-cancel", "Wait, no nevermind.");
    public TranslatableString PanelConfirmExit => Get("panel-confirm-exit", "Are you sure you want to exit?");

    public TranslatableString MapBPM => Get("map-bpm", "BPM");
    public TranslatableString MapLength => Get("map-length", "Length");
    public TranslatableString MapRating => Get("map-rating", "Rating");
    public TranslatableString MapNotesPerSecond => Get("map-nps", "NPS");
    public TranslatableString MapHitObjects => Get("map-hits", "Hits");
    public TranslatableString MapLongNotes => Get("map-lns", "LNs");
    public TranslatableString MapLongNotePercent => Get("map-lnp", "LN%");
    public TranslatableString MapAccuracy => Get("map-acc", "Accuracy");
    public TranslatableString MapHealth => Get("map-hp", "Health");

    public TranslatableString StatusOnline => Get("status-online", "Online");
    public TranslatableString StatusOffline => Get("status-offline", "Offline");
    public TranslatableString StatusPlaying => Get("status-playing", "Playing");
    public TranslatableString StatusWatchingReplay => Get("status-watching-replay", "Watching Replay");
    public TranslatableString StatusEditing => Get("status-editing", "Editing");
    public TranslatableString StatusMultiplayer => Get("status-multiplayer", "Playing Multiplayer");

    public TranslatableString CanNotBeUndone => Get("can-not-be-undone", "This action cannot be undone.");

    public TranslatableString LoginToUse => Get("login-to-use", "Log in to use this feature.");
}
