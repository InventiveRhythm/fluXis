using System.Threading.Tasks;

namespace fluXis.Online.Spectator;

public interface ISpectatorClient
{
    Task StartedPlaying(long id, SpectatorState state);
    Task StoppedPlaying(long id, SpectatorState state);
    Task RecieveFrameBundle(long id, SpectatorFrameBundle bundle);
    Task ScoreProcessed(long id, SpectatorState state);
}
