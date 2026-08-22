using fluXis.Utils;

namespace fluXis.Plugins;

public partial class PluginManager : AssemblyLoader<Plugin>
{
    protected override string StorageFolder => "plugins";
    protected override string AssemblyPrefix => "fluXis.Import";

    protected override void SetupType(Plugin plugin) => plugin.CreateConfig(Storage);
}
