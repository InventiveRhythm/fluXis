using System.ComponentModel.DataAnnotations.Schema;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset;
using YamlDotNet.Serialization;

namespace fluXis.Map.Structures;

#nullable enable

public abstract class HitObject : ITimedObject
{
    [YamlIgnore, NotMapped]
    public virtual int ComboContribution => 1;

    [YamlIgnore, NotMapped]
    public virtual float DensityContribution => 1;

    public double Time { get; set; }
    public int Lane { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Sample { get; set; } = string.Empty;

    /// <summary>
    /// The next HitObject in the same lane.
    /// </summary>
    [YamlIgnore, NotMapped]
    public HitObject? NextObject { get; set; }

    /// <summary>
    /// The scroll group for this object.
    /// </summary>
    [YamlIgnore, NotMapped]
    public ScrollGroup? ScrollGroup { get; set; }

    public virtual bool OnEditorMiddleClick() => false;
}
