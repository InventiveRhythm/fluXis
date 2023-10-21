using System.Globalization;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings;

public partial class TimingPointSettings : PointSettings<TimingPointInfo>
{
    private BasicPointSettingsField bpmField;
    private BasicPointSettingsField signatureField;

    public TimingPointSettings(TimingPointInfo point)
        : base(point)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            bpmField = new BasicPointSettingsField
            {
                Label = "BPM",
                Text = Point.BPM.ToStringInvariant(),
                OnTextChanged = saveChanges
            },
            signatureField = new BasicPointSettingsField
            {
                Label = "Signature",
                Text = Point.Signature.ToString(),
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

        if (!float.TryParse(bpmField.Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var bpm) || bpm <= 0)
        {
            bpmField.TextBox.NotifyError();
            return;
        }

        if (!int.TryParse(signatureField.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var signature) || signature <= 0)
        {
            signatureField.TextBox.NotifyError();
            return;
        }

        Point.Time = time;
        Point.BPM = bpm;
        Point.Signature = signature;

        Values.MapInfo.Change(Point);
    }
}
