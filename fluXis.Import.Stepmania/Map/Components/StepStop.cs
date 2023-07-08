using System.Collections.Generic;
using System.Globalization;

namespace fluXis.Import.Stepmania.Map.Components;

public class StepStop
{
    public float Beat { get; init; }
    public float Seconds { get; init; }

    public static List<StepStop> Parse(string line)
    {
        var stops = new List<StepStop>();

        var split = line.Split(',');

        foreach (var stop in split)
        {
            if (string.IsNullOrEmpty(stop))
                continue;

            var stopSplit = stop.Replace(",", "").Replace(";", "").Split('=');

            if (stopSplit.Length != 2)
                continue;

            stops.Add(new StepStop
            {
                Beat = float.Parse(stopSplit[0], CultureInfo.InvariantCulture),
                Seconds = float.Parse(stopSplit[1], CultureInfo.InvariantCulture)
            });
        }

        return stops;
    }
}
