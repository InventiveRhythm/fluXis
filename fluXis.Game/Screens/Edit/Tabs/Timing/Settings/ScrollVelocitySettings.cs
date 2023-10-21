using System.Globalization;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings;

public partial class ScrollVelocitySettings : PointSettings<ScrollVelocityInfo>
{
    private BasicPointSettingsField multiplierField;

    public ScrollVelocitySettings(ScrollVelocityInfo point)
        : base(point)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            multiplierField = new BasicPointSettingsField
            {
                Label = "Multiplier",
                Text = Point.Multiplier.ToStringInvariant(),
                OnTextChanged = saveChanges
            }
        });

        TimeField.OnTextChanged = saveChanges;
    }

    private void saveChanges()
    {
        if (!float.TryParse(TimeField.Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var time) || time < 0)
        {
            TimeField.TextBox.NotifyError();
            return;
        }

        if (!float.TryParse(multiplierField.Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var multiplier))
        {
            multiplierField.TextBox.NotifyError();
            return;
        }

        Point.Time = time;
        Point.Multiplier = multiplier;

        Values.MapInfo.Change(Point);
    }
}
