using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectPasteAction<T> : EditorAction
    where T : ITimedObject
{
    public override string Description => $"Paste {objs.Length} {ChartingTab.FormatTypeName<T>(objs.Length > 1)}";

    private T[] objs { get; }

    public ObjectPasteAction(T[] objs)
    {
        this.objs = objs;
    }

    public override void Run(EditorMap map)
    {
        foreach (var info in objs)
            map.Add(info);
    }

    public override void Undo(EditorMap map)
    {
        foreach (var info in objs)
            map.Remove(info);
    }
}
