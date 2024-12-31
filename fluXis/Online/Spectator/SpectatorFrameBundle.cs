using System.Collections.Generic;
using fluXis.Replays;
using MessagePack;
using Newtonsoft.Json;

namespace fluXis.Online.Spectator;

public class SpectatorFrameBundle
{
    [Key(1)]
    public IList<ReplayFrame> Frames { get; set; }

    /*public SpectatorFrameBundle(IList<ReplayFrame> frames)
    {
        this.Frames = frames;
    }*/

    [JsonConstructor]
    public SpectatorFrameBundle(IList<ReplayFrame> frames)
    {
        Frames = frames;
    }
}
