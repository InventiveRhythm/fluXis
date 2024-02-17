using System.IO;

namespace fluXis.Game.Updater;

/// <summary>
/// Manages updates.
/// </summary>
public interface IUpdatePerformer
{
    /// <summary>
    /// Performs an update. (If available)
    /// </summary>
    /// <param name="version">
    /// The version to update to.
    /// </param>
    public void Perform(string version);

    /// <summary>
    /// Installs an update from a file.
    /// </summary>
    /// <param name="file">
    /// The file to install from.
    /// </param>
    public void UpdateFromFile(FileInfo file);
}
