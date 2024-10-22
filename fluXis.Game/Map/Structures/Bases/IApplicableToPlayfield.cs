using fluXis.Game.Screens.Gameplay.Ruleset.Playfields;

namespace fluXis.Game.Map.Structures.Bases;

public interface IApplicableToPlayfield : ITimedObject
{
    void Apply(Playfield playfield);
}
