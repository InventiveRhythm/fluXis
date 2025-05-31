using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardNewsTab : DashboardWipTab
{
    public override LocalisableString Title => LocalizationStrings.Dashboard.News;
    public override IconUsage Icon => FontAwesome6.Solid.Newspaper;
    public override DashboardTabType Type => DashboardTabType.News;
}
