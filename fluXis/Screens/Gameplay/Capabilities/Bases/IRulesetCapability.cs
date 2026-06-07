using fluXis.Screens.Gameplay.Ruleset;

namespace fluXis.Screens.Gameplay.Capabilities.Bases;

#nullable enable

public interface IRulesetCapability : IGameplayCapability
{
    RulesetContainer? Create() => null;
    void Modify(RulesetContainer ruleset) { }
}
