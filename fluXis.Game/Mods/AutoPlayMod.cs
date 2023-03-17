namespace fluXis.Game.Mods;

public class AutoPlayMod : IMod
{
    public string Name => "AutoPlay";
    public string Acronym => "AP";
    public string Description => "Automatically plays the map for you.";
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => false;
}
