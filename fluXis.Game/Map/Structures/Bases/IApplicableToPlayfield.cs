using fluXis.Game.Screens.Gameplay.Ruleset;

namespace fluXis.Game.Map.Structures.Bases;

public interface IApplicableToPlayfield
{
    void Apply(Playfield playfield);
}
