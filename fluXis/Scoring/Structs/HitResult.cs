using System;
using fluXis.Scoring.Enums;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Scoring.Structs;

public readonly struct HitResult : IEquatable<HitResult>
{
    [JsonProperty("time")]
    public double Time { get; init; }

    [JsonProperty("difference")]
    public double Difference { get; init; }

    [JsonProperty("judgement")]
    public Judgement Judgement { get; init; }

    [JsonProperty("holdend")]
    public bool HoldEnd { get; init; }

    public HitResult(double time, double diff, Judgement jud, bool end)
    {
        Time = time;
        Difference = diff;
        Judgement = jud;
        HoldEnd = end;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR)]
    public HitResult()
    {
    }

    public bool Equals(HitResult other) => Time.Equals(other.Time) && Difference.Equals(other.Difference) && Judgement == other.Judgement;
    public override bool Equals(object obj) => obj is HitResult other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Time, Difference, (int)Judgement);
    public static bool operator ==(HitResult left, HitResult right) => left.Equals(right);
    public static bool operator !=(HitResult left, HitResult right) => !(left == right);
}
