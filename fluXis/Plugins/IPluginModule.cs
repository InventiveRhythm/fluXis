using JetBrains.Annotations;

namespace fluXis.Plugins;

/// <summary>
/// A component of a <see cref="Plugin"/> that modulizes specific logic or capabilities.
/// </summary>
/// <remarks>
/// Modules allow plugins to be split into smaller units. A module can provide functionality
/// by implementing various <see cref="IPluginCapability"/> interfaces.
/// <code>
/// public class SongSelectModule : IPluginModule, ISongSelectCapability
/// {
///     public (MenuItem Item, Func&lt;bool&gt; Predicate)[] MapSetContextMenuItems => new[]
///     {
///         (new MenuItem("Hello", () => Logger.Log("Hello, world!"), () => true)
///     };
///
///     public void OnMapChanged(RealmMap map)
///     {
///         Logger.Log($"Selected Song: {map.Metadata.Title}");
///     }
/// }
/// </code>
/// </remarks>
[UsedImplicitly]
public interface IPluginModule
{
    public Plugin Plugin { get; internal set; }
    public PluginLogger Logger { get; internal set; }

    public T GetCapability<T>() where T : class, IPluginCapability
        => this as T;

    public bool HasCapability<T>() where T : class, IPluginCapability
        => this is T;
};
