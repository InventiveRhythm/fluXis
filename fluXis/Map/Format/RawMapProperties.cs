using System;
using fluXis.Online.API.Models.Maps;
using YamlDotNet.Serialization;

namespace fluXis.Map.Format;

public class RawMapProperties
{
    [YamlMember(Alias = "flux:difficulty_accuracy")]
    public float AccuracyDifficulty { get; set; } = 8;

    [YamlMember(Alias = "flux:difficulty_health")]
    public float HealthDifficulty { get; set; } = 8;

    [YamlMember(Alias = "flux:audio_visualizations")]
    public bool AudioVisualizations { get; set; }

    [YamlMember(Alias = "flux:dual")]
    public DualMode Dual { get; set; } = DualMode.Disabled;

    [YamlMember(Alias = "flux:force_aspect")]
    public bool ForceAspect { get; set; }

    [YamlMember(Alias = "flux:extra_playfields")]
    [field: YamlIgnore]
    public int ExtraPlayfields { get; set => field = Math.Clamp(value, 0, 9); }
}
