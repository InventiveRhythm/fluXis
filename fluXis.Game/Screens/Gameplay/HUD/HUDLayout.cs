using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Game.Screens.Gameplay.HUD;

public class HUDLayout
{
    public string Name { get; set; } = "New Layout";
    public string Author { get; set; } = "Me";

    [JsonIgnore]
    public string ID { get; set; }

    public Dictionary<string, HUDComponentSettings> Gameplay { get; set; }

    public override string ToString() => $"{Name} by {Author} ({ID})";
}
