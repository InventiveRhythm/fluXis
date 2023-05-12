using System.Globalization;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Timing;

public partial class PointSettings<T> : PointSettings
    where T : TimedObject
{
    public T Point { get; }

    public BasicPointSettingsField TimeField { get; }

    public PointSettings(T point)
    {
        Point = point;

        Add(TimeField = new BasicPointSettingsField
        {
            Label = "Time",
            Text = Point.Time.ToString(CultureInfo.InvariantCulture)
        });
    }
}

public partial class PointSettings : FillFlowContainer
{
    public TimingTab Tab { get; set; }

    public PointSettings()
    {
        Direction = FillDirection.Vertical;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Spacing = new Vector2(0, 10);
    }
}
