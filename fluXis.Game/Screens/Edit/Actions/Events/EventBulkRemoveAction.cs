using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures.Bases;

namespace fluXis.Game.Screens.Edit.Actions.Events;

public class EventBulkRemoveAction : EditorAction
{
    public override string Description => $"Remove {objs.Count()} events";

    private IEnumerable<ITimedObject> objs { get; }

    public EventBulkRemoveAction(IEnumerable<ITimedObject> objs)
    {
        this.objs = objs;
    }

    public override void Run(EditorMap map)
    {
        foreach (var obj in objs)
            map.Remove(obj);
    }

    public override void Undo(EditorMap map)
    {
        foreach (var info in objs)
            map.Add(info);
    }
}

