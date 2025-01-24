using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace fluXis.Online.Notifications;

public interface INotificationServer
{
    Task UpdateActivity(string name, JObject data);
}
