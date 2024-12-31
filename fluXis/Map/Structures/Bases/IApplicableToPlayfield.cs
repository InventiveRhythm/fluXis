using fluXis.Screens.Gameplay.Ruleset.Playfields;

namespace fluXis.Map.Structures.Bases;

public interface IApplicableToPlayfield : ITimedObject
{
    int PlayfieldIndex { get; set; }

    void Apply(Playfield playfield);
}
