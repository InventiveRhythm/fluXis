using System.Globalization;
using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings.Preset;

public partial class PointSettingsTime : PointSettingsTextBox
{
    public PointSettingsTime(EditorMapInfo info, TimedObject obj)
    {
        Text = "Time";
        DefaultText = obj.Time.ToStringInvariant();
        OnTextChanged = box =>
        {
            if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var time))
            {
                obj.Time = time;
                info.Update(obj);
            }
            else TextBox.NotifyError();
        };
    }
}
