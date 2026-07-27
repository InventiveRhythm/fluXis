using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectPlaceAction<T> : EditorAction
    where T : ITimedObject
{
    public override string Description => $"Place {ChartingTab.FormatTypeName<T>()} at {(int)obj.Time}ms on lane {obj.Lane}";
    private T obj { get; }

    public ObjectPlaceAction(T obj)
    {
        this.obj = obj;
    }

    public override void Run(EditorMap map) => map.Add(obj);
    public override void Undo(EditorMap map) => map.Remove(obj);
}
