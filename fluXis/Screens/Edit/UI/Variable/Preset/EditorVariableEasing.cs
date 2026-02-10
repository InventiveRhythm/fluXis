using System;
using System.Linq;
using fluXis.Map.Structures.Bases;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.Variable.Preset;

public partial class EditorVariableEasing<T> : EditorVariableDropdown<Easing>
    where T : class, ITimedObject, IHasEasing
{
    public EditorVariableEasing(EditorMap map, T obj)
    {
        Text = "Easing";
        TooltipText = "The easing function used to interpolate between scales.";
        Items = Enum.GetValues<Easing>().ToList();
        CurrentValue = obj.Easing;
        OnValueChanged = easing =>
        {
            obj.Easing = easing;
            map.Update(obj);
        };
    }
}
