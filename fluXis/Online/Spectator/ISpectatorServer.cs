using System.Threading.Tasks;
using fluXis.Online.Spectator.Models;
using JetBrains.Annotations;

namespace fluXis.Online.Spectator;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface ISpectatorServer
{
    // player
    Task StartSession(SpectatorState state);
    Task SendFrameBundle(SpectatorFrameBundle bundle);
    Task EndSession();

    // viewer
    Task StartWatching(long id);
    Task StopWatching(long id);
}
