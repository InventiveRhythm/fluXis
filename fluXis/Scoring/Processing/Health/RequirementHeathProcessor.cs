using fluXis.Map;
using fluXis.Scoring.Structs;

namespace fluXis.Scoring.Processing.Health;

public class RequirementHeathProcessor : HealthProcessor
{
    protected override double DefaultHealth => 0f;
    protected override bool ClearHealthOnFail => false;

    public float HealthRequirement { get; init; }
    public bool RequirementReached => Health.Value >= HealthRequirement;

    private float multiplier = 1f;
    private const float miss_multiplier = 0.4f;

    public RequirementHeathProcessor(float difficulty)
        : base(difficulty)
    {
    }

    public override void ApplyMap(MapInfo map, int playerIndex)
    {
        multiplier = 1f / (map.MaxComboForPlayer(playerIndex) * 0.05f);
        multiplier *= 100f;
    }

    public override void AddResult(HitResult result)
    {
        Health.Value += GetHealthIncreaseFor(result, Difficulty);
    }

    protected override float GetHealthIncreaseFor(HitResult result, float difficulty)
    {
        var increase = base.GetHealthIncreaseFor(result, difficulty);

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
