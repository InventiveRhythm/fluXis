using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Activity;

public interface IActivity
{
    void Initialize();
    void OnRemove();
    void Update(Fluxel fluxel, string details = "", string state = "", string largeImageKey = "", int timestamp = 0, int timeLeft = 0);
}
