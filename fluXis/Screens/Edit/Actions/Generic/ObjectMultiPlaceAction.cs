using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectMultiPlaceAction<T> : EditorAction
    where T : ITimedObject
{
    public override string Description => $"Place {objs.Length} {ChartingTab.FormatTypeName<T>(objs.Length > 1)} at {(int)objs[0].Time}ms";
    private T[] objs { get; }

    public ObjectMultiPlaceAction(T[] objs)
    {
        this.objs = objs;
    }

    public override void Run(EditorMap map) => objs.ForEach(x => map.Add(x));
    public override void Undo(EditorMap map) => objs.ForEach(x => map.Remove(x));
}
