using fluXis.Map.Structures.Bases;

namespace fluXis.Screens.Edit.UI.Variable.Preset;

public partial class EditorVariableLength<T> : EditorVariableBeats<T>
    where T : class, ITimedObject, IHasDuration
{
    protected override double Value
    {
        get => Object.Duration;
        set => Object.Duration = value;
    }

    public EditorVariableLength(EditorMap map, T obj, float beatLength)
        : base(map, obj, beatLength)
    {
        Text = "Animation Length";
        TooltipText = "The duration of the animation in beats.";
    }
}
