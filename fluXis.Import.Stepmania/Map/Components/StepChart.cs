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

    public int GetKeyCount()
    {
        // from https://github.com/etternagame/etterna/blob/2d706147cc4bcf95a8ceb24dc4f4129be207f803/src/Etterna/Singletons/GameManager.cpp#L45
        // add more as needed?
        return Type?.Trim().ToLowerInvariant() switch
        {
            "dance-single" => 4,
            "dance-double" => 8,
            "dance-solo" => 6,
            "pump-single" => 5,
            "pump-halfdouble" => 6,
            "pump-double" => 10,
            _ => 4
        };
    }
}
