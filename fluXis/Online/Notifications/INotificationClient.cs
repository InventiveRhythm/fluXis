using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Notifications;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Collections;
using JetBrains.Annotations;

namespace fluXis.Online.Notifications;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface INotificationClient
{
    Task Login(APIUser user);
    Task Logout(string reason);

    Task NotificationReceived(APINotification notification);
    Task NotifyFriendStatus(APIUser friend, bool online);

    Task RewardAchievement(Achievement achievement);
    Task DisplayMessage(ServerMessage message);
    Task DisplayMaintenance(long time);
    Task ForceNameChange();

    Task ReceiveChatMessage(APIChatMessage message);
    Task DeleteChatMessage(string channel, string id);
    Task AddToChatChannel(APIChatChannel channel);
    Task RemoveFromChatChannel(string channel);

    Task CollectionUpdated(string id, List<CollectionItem> added, List<CollectionItem> changed, List<string> removed);
}
