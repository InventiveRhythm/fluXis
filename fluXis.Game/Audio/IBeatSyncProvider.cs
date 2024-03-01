using System;

namespace fluXis.Game.Audio;

public interface IBeatSyncProvider
{
    double StepTime { get; }
    double BeatTime { get; }
    Action<int> OnBeat { get; set; }
}
