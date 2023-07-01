using System;
using Newtonsoft.Json;

namespace fluXis.Game.Skinning.Json;

public class Skin
{
    [JsonProperty("1k")]
    public SkinKeymode OneKey { get; set; } = new() { ColumnWidth = 132 };

    [JsonProperty("2k")]
    public SkinKeymode TwoKey { get; set; } = new() { ColumnWidth = 126 };

    [JsonProperty("3k")]
    public SkinKeymode ThreeKey { get; set; } = new() { ColumnWidth = 120 };

    [JsonProperty("4k")]
    public SkinKeymode FourKey { get; set; } = new() { ColumnWidth = 114 };

    [JsonProperty("5k")]
    public SkinKeymode FiveKey { get; set; } = new() { ColumnWidth = 108 };

    [JsonProperty("6k")]
    public SkinKeymode SixKey { get; set; } = new() { ColumnWidth = 102 };

    [JsonProperty("7k")]
    public SkinKeymode SevenKey { get; set; } = new() { ColumnWidth = 96 };

    [JsonProperty("8k")]
    public SkinKeymode EightKey { get; set; } = new() { ColumnWidth = 90 };

    [JsonProperty("9k")]
    public SkinKeymode NineKey { get; set; } = new() { ColumnWidth = 84 };

    [JsonProperty("10k")]
    public SkinKeymode TenKey { get; set; } = new() { ColumnWidth = 78 };

    public SkinKeymode GetKeymode(int keyCount)
    {
        return keyCount switch
        {
            1 => OneKey,
            2 => TwoKey,
            3 => ThreeKey,
            4 => FourKey,
            5 => FiveKey,
            6 => SixKey,
            7 => SevenKey,
            8 => EightKey,
            9 => NineKey,
            10 => TenKey,
            _ => throw new ArgumentOutOfRangeException(nameof(keyCount), keyCount, null)
        };
    }

    public Skin Copy() => (Skin)MemberwiseClone();
}
