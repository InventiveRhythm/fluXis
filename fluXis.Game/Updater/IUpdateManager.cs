using System.IO;

namespace fluXis.Game.Updater;

/// <summary>
/// Manages updates.
/// </summary>
public interface IUpdateManager
{
    /// <summary>
    /// Whether an update is available.
    /// </summary>
    public bool UpdateAvailable { get; }

    /// <summary>
    /// Performs an update. (If available)
    /// </summary>
    /// <param name="silent">
    /// Whether to show a notification if no updates are available.
    /// </param>
    /// <param name="forceUpdate">
    /// Ignores if you are already on the latest version and installs the latest version.
    /// </param>
    public void Perform(bool silent, bool forceUpdate = false);

    /// <summary>
    /// Installs an update from a file.
    /// </summary>
    /// <param name="file">
    /// The file to install from.
    /// </param>
    public void UpdateFromFile(FileInfo file);
}
