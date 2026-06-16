using fluXis.Import;
using JetBrains.Annotations;

namespace fluXis.Plugins.Capabilities;

[UsedImplicitly]
public interface IMapImporterCapability : IPluginCapability
{
    [CanBeNull]
    MapImporter Importer { get; }
}
