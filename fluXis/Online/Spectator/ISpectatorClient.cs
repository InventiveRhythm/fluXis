using System.Threading.Tasks;
using fluXis.Online.Spectator.Models;
using JetBrains.Annotations;

namespace fluXis.Online.Spectator;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface ISpectatorClient
{
    Task StartedPlaying(long id, SpectatorState state);
    Task ReceiveFrameBundle(long id, SpectatorFrameBundle bundle);
    Task StoppedPlaying(long id);
}
