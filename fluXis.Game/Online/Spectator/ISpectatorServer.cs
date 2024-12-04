using System.Threading.Tasks;

namespace fluXis.Game.Online.Spectator;

public interface ISpectatorServer
{
    Task StartSession(long score, SpectatorState state);
    Task SendFrameBundle();
    Task EndSession(SpectatorState state);

    Task StartWatching(long id);
    Task StopWatching(long id);
}
