using System.Collections.Generic;
using fluXis.Game.Replays;
using MessagePack;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Spectator;

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
