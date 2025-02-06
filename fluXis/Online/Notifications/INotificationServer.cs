using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace fluXis.Online.Notifications;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface INotificationServer
{
    Task UpdateActivity(string name, JObject data);
}
