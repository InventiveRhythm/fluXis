using fluXis.Screens.Gameplay.Ruleset.Playfields;

namespace fluXis.Map.Structures.Bases;

public interface IApplicableToPlayfield : ITimedObject
{
    int PlayfieldIndex { get; set; }
    int PlayfieldSubIndex { get; set; }

    void Apply(Playfield playfield);
}

public static class ApplicableToPlayfieldExtensions
{
    public static bool AppliesTo(this IApplicableToPlayfield ev, Playfield playfield)
    {
        if (ev.PlayfieldIndex != 0 && playfield.Index + 1 != ev.PlayfieldIndex)
            return false;

        return ev.PlayfieldSubIndex == 0 || playfield.SubIndex + 1 == ev.PlayfieldSubIndex;
    }
}
