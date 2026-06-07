using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Midori.Utils;

namespace fluXis.Replays;

public class ReplayFrame
{
    public const float SYNC_INTERVAL = 1000;

    public double Time { get; set; }
    public ReplayFrameType Type { get; set; }

    [CanBeNull]
    public List<int> Actions { get; init; }

    public ReplayFrame(double time, params int[] actions)
    {
        Time = time;
        Actions = new List<int>(actions);
        Type = ReplayFrameType.Input;
    }

    public ReplayFrame(double time)
    {
        Time = time;
        Type = ReplayFrameType.Sync;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR)]
    public ReplayFrame()
    {
    }
}

public enum ReplayFrameType
{
    Input,
    Sync
}
