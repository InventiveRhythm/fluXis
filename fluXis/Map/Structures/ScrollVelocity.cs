using System;
using System.Collections.Generic;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset;
using Newtonsoft.Json;

namespace fluXis.Map.Structures;

public class ScrollVelocity : ITimedObject, IHasGroups
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("multiplier")]
    public double Multiplier { get; set; } = 1;

    [JsonProperty("groups", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string> Groups { get; set; } = new();

    [JsonProperty("mask")]
    [Obsolete($"Use {nameof(ScrollVelocity)}.{nameof(Groups)} instead.")]
    public List<bool> LaneMask
    {
        set
        {
            for (var i = 0; i < value.Count; i++)
            {
                if (value[i])
                    Groups.Add($"${i + 1}");
            }
        }
    }

    public void Apply(ScrollGroup group) => group.AddVelocity(this);
}
