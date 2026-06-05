using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Replays;

public class Replay
{
    public long PlayerID { get; set; } = -1;
    public List<ReplayFrame> Frames { get; set; } = new();

    [JsonIgnore]
    public double LastSync { get; set; }
}
