using System;
using System.Linq;
using fluXis.Game.Map.Structures.Bases;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsEasing<T> : PointSettingsDropdown<Easing>
    where T : class, ITimedObject, IHasEasing
{
    public PointSettingsEasing(EditorMap map, T obj)
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
