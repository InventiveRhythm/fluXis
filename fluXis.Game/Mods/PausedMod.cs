namespace fluXis.Game.Mods;

public class PausedMod : IMod
{
    public string Name => "Paused";
    public string Acronym => "PA";
    public string Description => "Paused the game mid-play.";
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => false;
}
