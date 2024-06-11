using System.Globalization;
using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsTime : PointSettingsTextBox
{
    public PointSettingsTime(EditorMap map, ITimedObject obj)
    {
        Text = "Time";
        TooltipText = "The time in milliseconds when the event should trigger.";
        DefaultText = obj.Time.ToStringInvariant("0");
        OnTextChanged = box =>
        {
            if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var time))
            {
                obj.Time = time;
                map.Update(obj);
            }
            else TextBox.NotifyError();
        };
    }
}
