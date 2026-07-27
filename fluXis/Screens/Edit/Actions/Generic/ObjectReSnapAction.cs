using System;
using System.Collections.Generic;
using fluXis.Map.Structures.Bases;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectReSnapAction : EditorAction
{
    public override string Description => "Re-snap all objects.";

    private IEnumerable<ITimedObject> objs { get; }
    private Func<double, double> snapTime { get; }
    private int snapDivisor { get; }

    private Dictionary<ITimedObject, double> oldTimes { get; } = new();

    public ObjectReSnapAction(List<ITimedObject> objs, Func<double, double> snapTime, int snapDivisor)
    {
        this.objs = objs;
        this.snapTime = snapTime;
        this.snapDivisor = snapDivisor;

        foreach (var note in objs)
            oldTimes[note] = note.Time;
    }

    public override void Run(EditorMap map)
    {
        foreach (var note in objs)
        {
            var tp = map.MapInfo.GetTimingPoint(note.Time);
            float increase = tp.Signature * tp.MsPerBeat / (4 * snapDivisor);

            var lower = snapTime(note.Time);
            var upper = snapTime(note.Time + increase);

            var lowerDiff = Math.Abs(note.Time - lower);
            var upperDiff = Math.Abs(note.Time - upper);

            note.Time = lowerDiff < upperDiff ? lower : upper;
        }
    }

    public override void Undo(EditorMap map)
    {
        foreach (var note in objs)
            note.Time = oldTimes[note];
    }
}
