using System.Collections.Generic;
using System.Linq;

namespace fluXis.Import.Stepmania.Map.Components;

public class StepMeasure
{
    public List<List<StepNote>> Notes { get; set; }

    public StepMeasure(List<List<StepNote>> notes) => Notes = notes;

    public static List<StepNote> Parse(string data)
    {
        var notes = new StepNote[4];

        for (var i = 0; i < 4; i++)
            notes[i] = Parse(data[i]);

        return notes.ToList();
    }

    public static StepNote Parse(char data)
    {
        return data switch
        {
            '0' => StepNote.None,
            '1' => StepNote.Normal,
            '2' => StepNote.Head,
            '3' => StepNote.Tail,
            '4' => StepNote.Roll,
            'M' => StepNote.Mine,
            _ => StepNote.None
        };
    }
}
