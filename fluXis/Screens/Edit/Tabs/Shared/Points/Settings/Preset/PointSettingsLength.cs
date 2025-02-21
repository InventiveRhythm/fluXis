using fluXis.Map.Structures.Bases;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsLength<T> : PointSettingsBeats<T>
    where T : class, ITimedObject, IHasDuration
{
    protected override double Value
    {
        get => Object.Duration;
        set => Object.Duration = value;
    }

    public PointSettingsLength(EditorMap map, T obj, float beatLength)
        : base(map, obj, beatLength)
    {
        Text = "Animation Length";
        TooltipText = "The duration of the animation in beats.";
    }
}
