using System;
using System.Linq;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsEasing : PointSettingsDropdown<Easing>
{
    public PointSettingsEasing()
    {
        Text = "Easing";
        TooltipText = "The easing function used to interpolate between scales.";
        Items = Enum.GetValues<Easing>().ToList();
    }
}
