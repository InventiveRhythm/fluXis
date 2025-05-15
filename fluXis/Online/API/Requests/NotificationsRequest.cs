using fluXis.Online.API.Models.Notifications;

namespace fluXis.Online.API.Requests;

public class NotificationsRequest : APIRequest<APINotificationList>
{
    protected override string Path => "/social/notifications";
}
