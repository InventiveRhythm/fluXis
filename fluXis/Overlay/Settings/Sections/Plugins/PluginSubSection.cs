using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Overlay.Settings.Sections.Plugins.Import;
using fluXis.Plugins;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Plugins;

public partial class PluginSubSection : SettingsSubSection
{
    private readonly PluginType type;
    private int pluginCount;

    public PluginSubSection(PluginType type)
    {
        this.type = type;
    }

    public override LocalisableString Title => $"{type} Plugins";
    public override IconUsage Icon => Phosphor.Bold.Plug;

    [BackgroundDependencyLoader]
    private void load(PluginManager pluginManager)
    {
        var plugins = pluginManager.Plugins.Where(p => p.Type == type).ToArray();

        foreach (var plugin in plugins)
        {
            Add(plugin.Type switch
            {
                PluginType.Import => new DrawableImportPlugin { Plugin = plugin },
                _ => new DrawablePlugin { Plugin = plugin }
            });
        }

        pluginCount = plugins.Length;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        FluXisSpriteText noPluginsText;

        if (pluginCount != 0) return;

        Add(noPluginsText = new FluXisSpriteText
        {
            Text = "No Plugins Available.",
            Colour = Theme.Text2,
            WebFontSize = 20,
            Alpha = float.Epsilon
        });

        noPluginsText.FadeIn(300);
    }
}
