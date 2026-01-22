using System;
using System.Collections.Generic;
using fluXis.Map;
using fluXis.Scoring.Structs;

namespace fluXis.Scoring.Processing;

public class JudgementProcessor
{
    private readonly List<JudgementDependant> dependants = new();
    public List<HitResult> Results { get; } = new();

    public event Action<HitResult> ResultAdded;
    public event Action<HitResult> ResultReverted;

    public void AddDependants(IEnumerable<JudgementDependant> dependants)
    {
        foreach (var dependant in dependants)
        {
            dependant.JudgementProcessor = this;
            this.dependants.Add(dependant);
        }
    }

    public void ApplyMap(MapInfo map)
    {
        dependants.ForEach(d => d.ApplyMap(map));
    }

    public void AddResult(HitResult result)
    {
        lock (Results) Results.Add(result);

        dependants.ForEach(d => d.AddResult(result));
        ResultAdded?.Invoke(result);
    }

    public void RevertResult(HitResult result)
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
