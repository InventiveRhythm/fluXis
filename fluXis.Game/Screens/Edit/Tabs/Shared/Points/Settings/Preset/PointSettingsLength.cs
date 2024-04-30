using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsLength<T> : PointSettingsTextBox
    where T : ITimedObject, IHasDuration
{
    public PointSettingsLength(EditorMap map, T obj, float beatLength)
    {
        Text = "Animation Length";
        TooltipText = "The duration of the animation in beats.";
        ExtraText = "beat(s)";
        TextBoxWidth = 100;
        DefaultText = (obj.Duration / beatLength).ToStringInvariant();
        OnTextChanged = box =>
        {
            if (box.Text.TryParseFloatInvariant(out var result))
                obj.Duration = result * beatLength;
            else
                box.NotifyError();

            map.Update(obj);
        };
    }
}
