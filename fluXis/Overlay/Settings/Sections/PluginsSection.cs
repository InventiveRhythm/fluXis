using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Plugins;
using fluXis.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class PluginsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.PuzzlePiece;
    public override LocalisableString Title => LocalizationStrings.Settings.Plugins.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        Enum.GetValues<PluginType>().ForEach(ptype =>
        {
            Add(new PluginSubSection(ptype));
        });
    }
}
