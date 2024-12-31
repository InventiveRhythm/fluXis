using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures.Bases;

namespace fluXis.Screens.Edit.Actions.Events;

public class EventBulkCloneAction : EditorAction
{
    public override string Description => $"Clone {objs.Count()} events";

    private IEnumerable<ITimedObject> objs { get; }

    public EventBulkCloneAction(IEnumerable<ITimedObject> objs)
    {
        this.objs = objs;
    }

    public override void Run(EditorMap map)
    {
        foreach (var obj in objs)
            map.Add(obj);
    }

    public override void Undo(EditorMap map)
    {
        foreach (var info in objs)
            map.Remove(info);
    }
}
