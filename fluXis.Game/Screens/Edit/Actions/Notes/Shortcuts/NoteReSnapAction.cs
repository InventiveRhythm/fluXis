using System;
using System.Collections.Generic;
using fluXis.Game.Map.Structures;
using osu.Framework.Logging;

namespace fluXis.Game.Screens.Edit.Actions.Notes.Shortcuts;

public class NoteReSnapAction : EditorAction
{
    public override string Description => "Re-snap all notes";

    private IEnumerable<HitObject> notes { get; }
    private Func<double, double> snapTime { get; }
    private int snapDivisor { get; }

    private Dictionary<HitObject, double> oldTimes { get; } = new();

    public NoteReSnapAction(List<HitObject> notes, Func<double, double> snapTime, int snapDivisor)
    {
        this.notes = notes;
        this.snapTime = snapTime;
        this.snapDivisor = snapDivisor;

        foreach (var note in notes)
            oldTimes[note] = note.Time;
    }

    public override void Run(EditorMap map)
    {
        foreach (var note in notes)
        {
            var tp = map.MapInfo.GetTimingPoint(note.Time);
            float increase = tp.Signature * tp.MsPerBeat / (4 * snapDivisor);

            var lower = snapTime(note.Time);
            var upper = snapTime(note.Time + increase);

            var lowerDiff = Math.Abs(note.Time - lower);
            var upperDiff = Math.Abs(note.Time - upper);

            Logger.Log($"Time: {note.Time}, Lower: {lower}, Upper: {upper}, LowerDiff: {lowerDiff}, UpperDiff: {upperDiff}");
            note.Time = lowerDiff < upperDiff ? lower : upper;
        }
    }

    public override void Undo(EditorMap map)
    {
        foreach (var note in notes)
            note.Time = oldTimes[note];
    }
}
