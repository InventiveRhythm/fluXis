using System;

namespace fluXis.Game.Plugins;

public class PluginInfo
{
    public string Name { get; set; }
    public string Author { get; set; }
    public Version Version { get; set; }

    public override string ToString() => $"{Name} v{Version} by {Author}";
}
