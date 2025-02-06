using System.Threading.Tasks;
using JetBrains.Annotations;

namespace fluXis.Online.Spectator;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface ISpectatorServer
{
    Task StartSession(long score, SpectatorState state);
    Task SendFrameBundle(SpectatorFrameBundle bundle);
    Task EndSession(SpectatorState state);

    Task StartWatching(long id);
    Task StopWatching(long id);
}
