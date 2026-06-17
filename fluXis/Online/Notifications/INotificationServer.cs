using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace fluXis.Online.Notifications;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface INotificationServer
{
    #region Activities

    /// <summary>
    /// Update the current user's activity. Notifies all subscribed users of this change.
    /// </summary>
    Task UpdateActivity(string name, JObject data);

    /// <summary>
    /// Subscribe to a user's activity changes. Fires <see cref="INotificationClient.NotifyUserActivity"/>.
    /// </summary>
    Task SubscribeToUser(long id);

    #endregion

    Task UpdateNotificationUnread(long time);
}
