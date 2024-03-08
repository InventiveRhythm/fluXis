using fluXis.Game.Map;
using fluXis.Shared.Scoring.Structs;

namespace fluXis.Game.Scoring.Processing.Health;

public class RequirementHeathProcessor : HealthProcessor
{
    protected override float DefaultHealth => 0f;
    protected override bool ClearHealthOnFail => false;

    public float HealthRequirement { get; init; }
    public bool RequirementReached => Health.Value >= HealthRequirement;

    private float multiplier = 1f;
    private const float miss_multiplier = 0.4f;

    public override void ApplyMap(MapInfo map)
    {
        multiplier = 1f / (map.MaxCombo * 0.05f);
        multiplier *= 100f;
    }

    public override void AddResult(HitResult result)
    {
        Health.Value += GetHealthIncreaseFor(result);
    }

    protected override float GetHealthIncreaseFor(HitResult result)
    {
        var increase = base.GetHealthIncreaseFor(result);

        if (increase >= 0)
            increase *= multiplier;
        else
            increase *= miss_multiplier;

        return increase;
    }

    public override bool OnComplete()
    {
        if (!RequirementReached)
            TriggerFailure();

        return RequirementReached;
    }
}
