using System.Threading.Tasks;
using JetBrains.Annotations;

namespace fluXis.Online.Spectator;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface ISpectatorClient
{
    Task StartedPlaying(long id, SpectatorState state);
    Task StoppedPlaying(long id, SpectatorState state);
    Task RecieveFrameBundle(long id, SpectatorFrameBundle bundle);
    Task ScoreProcessed(long id, SpectatorState state);
}
