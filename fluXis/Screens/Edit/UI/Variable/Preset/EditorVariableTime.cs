using System;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.Variable.Preset;

#nullable enable

public partial class EditorVariableTime : EditorVariableNumber<double>
{
    [Resolved(CanBeNull = true)]
    private EditorSnapProvider? snaps { get; set; }

    private EditorMap map { get; }
    private ITimedObject obj { get; }
    private Func<double> getOffset { get; }

    public Action<double, double>? TimeChanged { get; init; }

    public EditorVariableTime(EditorMap map, ITimedObject obj, Func<double>? getOffset = null)
    {
        this.map = map;
        this.obj = obj;
        this.getOffset = getOffset ?? (() => 0);

        Text = "Time";
        TooltipText = "The time in milliseconds when the event should trigger.";
        CurrentValue = obj.Time;
        FetchStepValue = () => snaps?.CurrentStep ?? 1;
        OnValueChanged = v =>
        {
            var old = obj.Time;
            obj.Time = v;
            TimeChanged?.Invoke(old, v);
            map.Update(obj);
        };
    }

    protected override Drawable CreateExtraButton() => new EditorVariableToCurrentButton
    {
        Action = t =>
        {
            t -= getOffset();
            var old = obj.Time;
            CurrentValue = obj.Time = t;
            TimeChanged?.Invoke(old, t);
            map.Update(obj);
        }
    };
}
