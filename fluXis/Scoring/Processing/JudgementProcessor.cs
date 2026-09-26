using System;
using System.Collections.Generic;
using fluXis.Map;
using fluXis.Modes.Gameplay.Objects;

namespace fluXis.Scoring.Processing;

public class JudgementProcessor
{
    private readonly List<JudgementDependant> dependants = new();
    public List<TrackedHitObject> Results { get; } = new();

    public event Action<TrackedHitObject> ResultAdded;
    public event Action<TrackedHitObject> ResultReverted;

    public void AddDependants(IEnumerable<JudgementDependant> dependants)
    {
        foreach (var dependant in dependants)
        {
            dependant.JudgementProcessor = this;
            this.dependants.Add(dependant);
        }
    }

    public void ApplyMap(PlayableMap map)
    {
        dependants.ForEach(d => d.ApplyMap(map));
    }

    public void AddResult(TrackedHitObject result)
    {
        lock (Results) Results.Add(result);

        dependants.ForEach(d => d.AddResult(result));
        ResultAdded?.Invoke(result);
    }

    public void RevertResult(TrackedHitObject result)
    {
        lock (Results) Results.Remove(result);

        dependants.ForEach(d => d.RevertResult(result));
        ResultReverted?.Invoke(result);
    }

    public void RunLocked(Action act)
    {
        lock (Results) act?.Invoke();
    }
}
