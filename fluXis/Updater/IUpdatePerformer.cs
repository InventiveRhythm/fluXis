#if VELOPACK_BUILD

namespace fluXis.Updater;

/// <summary>
/// Manages updates.
/// </summary>
public interface IUpdatePerformer
{
    /// <summary>
    /// Performs an update. (If available)
    /// </summary>
    /// <param name="silent">
    /// Whether to show a notification when there is no new update.
    /// </param>
    /// <param name="beta">
    /// Whether to include beta versions.
    /// </param>
    public void Perform(bool silent, bool beta);
}

#endif
