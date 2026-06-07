namespace fluXis.Screens.Gameplay.Capabilities;

public interface IGameplayCapability
{
    GameplayScreen Screen { get; set; }

    /// <summary>
    /// Before load()
    /// </summary>
    void PreLoad() { }

    /// <summary>
    /// After load()
    /// </summary>
    void PostLoad() { }

    /// <summary>
    /// During OnExit()
    /// </summary>
    void Exit() { }
}
