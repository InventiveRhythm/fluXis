using System;
using System.Linq;
using System.Reflection;
using fluXis.Screens.Edit;
using fluXis.Screens.Edit.UI.Variable;

namespace fluXis.Utils.Inspect;

#nullable enable

public static class ObjectInspectExt
{
    public static EditorVariableBase? CreateVariableControl<T>(this ObjectProperty prop, T obj, EditorMap map)
    {
        EditorVariableBase? v = null;

        if (prop.CustomCreateMethod != null)
            return prop.CustomCreateMethod.Call(prop, obj, map) as EditorVariableBase;

        if (prop.Type.IsEnum)
        {
            var getValues = typeof(Enum)
                            .GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Where(x => x.Name == nameof(Enum.GetValues))
                            .First(x => x.IsGenericMethod);

            var values = getValues.MakeGenericMethod(prop.Type).Invoke(null, []);
            var dropdown = typeof(EditorVariableDropdown<>).MakeGenericType(prop.Type);

            v = (EditorVariableBase)Activator.CreateInstance(dropdown)!;

            var items = dropdown.GetProperty(nameof(EditorVariableDropdown<object>.Items), BindingFlags.Public | BindingFlags.Instance)!;
            items.SetValue(v, values);
        }
        else if (prop.Type == typeof(string))
            v = new EditorVariableTextBox();
        else if (prop.Type == typeof(bool))
            v = new EditorVariableToggle();

        v?.AssignProperty(prop, obj);
        return v;
    }
}
