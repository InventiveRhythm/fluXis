using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings;

public partial class SettingsSection : FillFlowContainer
{
    public virtual IconUsage Icon => FontAwesome6.Solid.Gear;
    public virtual LocalisableString Title => "Section";

    public IEnumerable<SettingsSubSection> SubSections => InternalChildren.OfType<SettingsSubSection>().ToList();

    protected static SettingsDivider Divider => new();

    protected SettingsSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);
        Alpha = 0;
    }
}
