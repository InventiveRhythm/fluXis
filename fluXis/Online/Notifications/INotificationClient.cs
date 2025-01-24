using System.Threading.Tasks;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;

namespace fluXis.Online.Notifications;

public interface INotificationClient
{
    Task Login(APIUser user);
    Task Logout(string reason);

    Task NotifyFriendStatus(APIUser friend, bool online);

    Task RewardAchievement(Achievement achievement);
    Task DisplayMessage(ServerMessage message);
    Task DisplayMaintenance(long time);

    Task ReceiveChatMessage(APIChatMessage message);
    Task DeleteChatMessage(string channel, string id);
    Task AddToChatChannel(string channel);
    Task RemoveFromChatChannel(string channel);
}
