using System;
using System.Collections.Generic;
using System.Globalization;
using fluXis.Game.Map.Events;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Map;

public class MapEvents
{
    public List<LaneSwitchEvent> LaneSwitchEvents = new();
    public List<FlashEvent> FlashEvents = new();

    public MapEvents Load(string content)
    {
        var lines = content.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            int index = line.IndexOf('(');
            int index2 = line.IndexOf(')');
            if (index == -1 || index2 == -1) continue;

            var type = line[..index];
            var args = line[(index + 1)..index2].Split(',');

            switch (type)
            {
                case "LaneSwitch":
                {
                    var laneSwitch = new LaneSwitchEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Count = int.Parse(args[1])
                    };

                    Logger.Log($"LaneSwitch: {laneSwitch.Time} {laneSwitch.Count}");

                    if (args.Length > 2)
                        laneSwitch.Speed = float.Parse(args[2], CultureInfo.InvariantCulture);

                    LaneSwitchEvents.Add(laneSwitch);
                    break;
                }

                case "Flash":
                    if (args.Length < 8) continue;

                    float duration = float.Parse(args[1], CultureInfo.InvariantCulture);
                    bool inBackground = args[2] == "true";
                    Easing easing = (Easing)Enum.Parse(typeof(Easing), args[3]);
                    Colour4 startColor = Colour4.FromHex(args[4]);
                    float startOpacity = float.Parse(args[5], CultureInfo.InvariantCulture);
                    Colour4 endColor = Colour4.FromHex(args[6]);
                    float endOpacity = float.Parse(args[7], CultureInfo.InvariantCulture);

                    FlashEvents.Add(new FlashEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = duration,
                        InBackground = inBackground,
                        Easing = easing,
                        StartColor = startColor,
                        StartOpacity = startOpacity,
                        EndColor = endColor,
                        EndOpacity = endOpacity
                    });
                    break;
            }
        }

        return this;
    }

    public string Save()
    {
        var content = string.Empty;
        foreach (var laneSwitch in LaneSwitchEvents) content += laneSwitch + Environment.NewLine;
        foreach (var flash in FlashEvents) content += flash + Environment.NewLine;
        return content;
    }
}
