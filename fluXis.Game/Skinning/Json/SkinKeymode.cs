using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Game.Skinning.Json;

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

    [JsonProperty("colors")]
    public List<string> Colors { get; set; } = new();

    [JsonProperty("receptors_first")]
    public bool ReceptorsFirst { get; set; }
}
