using fluXis.Map;
using fluXis.Modes.Gameplay.Objects;

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

    public override void ApplyMap(PlayableMap map)
    {
        multiplier = 1f / (map.MaxCombo * 0.05f);
        multiplier *= 100f;
    }

    public override void AddResult(TrackedHitObject result)
    {
        Health.Value += GetHealthIncreaseFor(result, Difficulty);
    }

    protected override float GetHealthIncreaseFor(TrackedHitObject result, float difficulty)
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
