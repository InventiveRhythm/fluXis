namespace fluXis.Game.Mods;

public interface IMod
{
    string Name { get; }
    string Acronym { get; }
    string Description { get; }

    float ScoreMultiplier { get; }
    bool Rankable { get; }
}
