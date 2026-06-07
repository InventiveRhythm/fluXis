using fluXis.Map;

namespace fluXis.Screens.Gameplay.Capabilities.Bases;

#nullable enable

public interface IMapCapability : IGameplayCapability
{
    MapInfo? Load() => null;
    void Modify(MapInfo map) { }
}
