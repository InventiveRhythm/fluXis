using System.Globalization;
using fluXis.Game.Map;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings;

public partial class ScrollVelocitySettings : PointSettings<ScrollVelocityInfo>
{
    public PointSettingsTextBox MultiplierTextBox { get; }

    public ScrollVelocitySettings(ScrollVelocityInfo point)
        : base(point)
    {
        Add(MultiplierTextBox = new PointSettingsTextBox("Multiplier", point.Multiplier.ToString(CultureInfo.InvariantCulture)));
    }

    public override void OnTextChanged()
    {
        if (float.TryParse(TimeTextBox.Text, out var time))
            Point.Time = time;

        if (float.TryParse(MultiplierTextBox.Text, out var multiplier))
            Point.Multiplier = multiplier;

        // Tab.OnScrollVelocityChanged();
    }
}
