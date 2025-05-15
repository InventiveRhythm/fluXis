using osu.Framework.Localisation;

namespace fluXis.Localization.Categories;

public class DashboardStrings : LocalizationCategory
{
    protected override string File => "dashboard";

    public TranslatableString Notifications => Get("notifications", "Notifications");
    public TranslatableString NotificationEmpty => Get("notifications-empty", "Nothing here....");

    public TranslatableString News => Get("news", "News");

    public TranslatableString Friends => Get("friends", "Friends");

    public TranslatableString Club => Get("club", "Club");

    public TranslatableString Online => Get("online", "Online");

    public TranslatableString Account => Get("account", "Account");
}
