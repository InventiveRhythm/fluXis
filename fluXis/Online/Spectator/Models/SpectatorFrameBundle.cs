using System;
using System.Collections.Generic;
using fluXis.Replays;
using Midori.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.Spectator.Models;

public class SpectatorFrameBundle
{
    public IList<ReplayFrame> Frames { get; set; }

    public SpectatorFrameBundle(IList<ReplayFrame> frames)
    {
        Frames = frames;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR)]
    public SpectatorFrameBundle()
    {
    }
}
