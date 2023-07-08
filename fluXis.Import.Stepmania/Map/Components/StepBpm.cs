using System.Collections.Generic;
using System.Globalization;

namespace fluXis.Import.Stepmania.Map.Components;

public class StepBpm
{
    public float Beat { get; init; }
    public float BPM { get; init; }

    public static List<StepBpm> Parse(string line)
    {
        var bpms = new List<StepBpm>();

        var split = line.Split(',');

        foreach (var bpm in split)
        {
            if (string.IsNullOrEmpty(bpm))
                continue;

            var bpmSplit = bpm.Replace(",", "").Replace(";", "").Split('=');

            if (bpmSplit.Length != 2)
                continue;

            bpms.Add(new StepBpm
            {
                Beat = float.Parse(bpmSplit[0], CultureInfo.InvariantCulture),
                BPM = float.Parse(bpmSplit[1], CultureInfo.InvariantCulture)
            });
        }

        return bpms;
    }
}
