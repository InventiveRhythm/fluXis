using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Plugins;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class PluginsSection : SettingsSection
{
    public override IconUsage Icon => Phosphor.Bold.PuzzlePiece;
    public override LocalisableString Title => LocalizationStrings.Settings.Plugins.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        var types = Enum.GetValues<PluginType>();

        for (var i = 0; i < types.Length; i++)
        {
            var ptype = types[i];

            Add(new PluginSubSection(ptype));
            if (i != types.Length - 1) Add(new SettingsDivider());
        }
    }
}
