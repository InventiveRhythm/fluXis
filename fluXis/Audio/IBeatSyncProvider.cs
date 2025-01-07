using System;

namespace fluXis.Audio;

public interface IBeatSyncProvider
{
    double StepTime { get; }
    double BeatTime { get; }
    Action<int> OnBeat { get; set; }
}
