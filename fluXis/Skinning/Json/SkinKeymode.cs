using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Skinning.Json;

public class SkinKeymode
{
    [JsonProperty("column_width")]
    public int ColumnWidth { get; set; } = 114;

    [JsonProperty("hit_position")]
    public int HitPosition { get; set; } = 130;

    /// <summary>
    /// tints the notes with the colors
    /// </summary>
    [JsonProperty("tint_notes")]
    public bool TintNotes { get; set; }

    /// <summary>
    /// tints the body and end of a long note.
    /// only applies when <see cref="TintNotes"/> is true.
    /// </summary>
    [JsonProperty("tint_lns")]
    public bool TintLongNotes { get; set; } = true;

    /// <summary>
    /// tints the receptors.
    /// only applies when <see cref="TintReceptors"/> is true.
    /// </summary>
    [JsonProperty("tint_receptors")]
    public bool TintReceptors { get; set; } = false;

    [JsonProperty("colors")]
    public List<string> Colors { get; set; } = new();

    [JsonProperty("receptors_first")]
    public bool ReceptorsFirst { get; set; }

    [JsonProperty("receptor_offset")]
    public int ReceptorOffset { get; set; }
}
