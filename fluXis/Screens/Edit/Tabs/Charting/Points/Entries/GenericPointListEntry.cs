using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Utils.Inspect;
using Midori.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class GenericPointListEntry<T> : PointListEntry
    where T : ITimedObject
{
    protected override string Text => ChartingTab.FormatTypeName<T>(title: true);
    protected override Colour4 Color => Theme.GetEventColor(Object, true);

    public GenericPointListEntry(T obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => Object.JsonCopy();

    protected override Drawable[] CreateValueContent() => [.. Object.CreateSidebarInfo()];

    protected override IEnumerable<Drawable> CreateSettings()
    {
        yield return new EditorVariableTitle(Text, () => Delete());

        var props = ObjectInspect.GetProperties(Object);

        foreach (var prop in props)
        {
            var draw = prop.CreateVariableControl(Object, Map);
            if (draw != null) yield return draw;
        }
    }
}
