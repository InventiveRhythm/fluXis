using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectRemoveAction<T> : EditorAction
    where T : ITimedObject
{
    public override string Description => $"Remove {objs.Length} {ChartingTab.FormatTypeName<T>(objs.Length > 1)}";
    private T[] objs { get; }

    public ObjectRemoveAction(T[] objs)
    {
        this.objs = objs;
    }

    public override void Run(EditorMap map)
    {
        foreach (var info in objs)
            map.Remove(info);
    }

    public override void Undo(EditorMap map)
    {
        foreach (var info in objs)
            map.Add(info);
    }
}
