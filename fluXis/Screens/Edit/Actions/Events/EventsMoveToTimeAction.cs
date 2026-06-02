using System.Collections.Generic;
using fluXis.Map.Structures.Bases;

namespace fluXis.Screens.Edit.Actions.Events;

public class EventsMoveToTimeAction : EditorAction
{
    public override string Description => $"Move {objects.Count} events to {destinationTime}ms.";

    private readonly List<ITimedObject> objects;
    private readonly double destinationTime;
    private readonly double sourceTime;

    public EventsMoveToTimeAction(List<ITimedObject> objects, double destinationTime)
    {
        this.objects = objects;
        this.destinationTime = destinationTime;
        this.sourceTime = objects[0].Time;
    }

    public override void Run(EditorMap map)
    {
        foreach (var o in objects)
        {
            double offset = o.Time - sourceTime;
            o.Time = destinationTime + offset;
            map.Update(o);
        }
    }

    public override void Undo(EditorMap map)
    {
        foreach (var o in objects)
        {
            double offset = o.Time - destinationTime;
            o.Time = sourceTime + offset;
            map.Update(o);
        }
    }
}
