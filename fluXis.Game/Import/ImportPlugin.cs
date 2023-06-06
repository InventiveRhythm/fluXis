using fluXis.Game.Plugins;

namespace fluXis.Game.Import;

public class ImportPlugin : PluginInfo
{
    public bool HasAutoImport { get; set; }
    public int ImporterId { get; set; }
}
