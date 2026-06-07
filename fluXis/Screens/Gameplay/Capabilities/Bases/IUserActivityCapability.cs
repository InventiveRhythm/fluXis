using fluXis.Online.Activity;

namespace fluXis.Screens.Gameplay.Capabilities.Bases;

#nullable enable

public interface IUserActivityCapability : IGameplayCapability
{
    UserActivity? Create() => null;
    void Modify(UserActivity activity) { }
}
