using System.Collections.Generic;

namespace fluXis.Import.Stepmania.Map.Components;

public class StepChart
{
    public string Type { get; set; }
    public string Description { get; set; }
    public string Difficulty { get; set; }
    public string Meter { get; set; }
    public string GrooveRadar { get; set; }

    public List<StepMeasure> Measures { get; set; } = new()
    {
        new StepMeasure(new List<List<StepNote>>())
    };
}
