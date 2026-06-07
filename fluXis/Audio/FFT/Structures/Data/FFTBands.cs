using NLua;

namespace fluXis.Audio.FFT.Structures.Data;

public readonly struct FFTBands
{
    [LuaMember(Name = "low")]
    public readonly float Low;

    [LuaMember(Name = "mid")]
    public readonly float Mid;

    [LuaMember(Name = "high")]
    public readonly float High;

    [LuaMember(Name = "total")]
    public readonly float Total;

    public FFTBands(float low, float mid, float high, float total)
    {
        Low = low;
        Mid = mid;
        High = high;
        Total = total;
    }

    [LuaMember(Name = "GetDominantBand")]
    public FFTBandType GetDominantBand()
    {
        if (Low >= Mid && Low >= High)
            return FFTBandType.Low;
        if (Mid >= Low && Mid >= High)
            return FFTBandType.Mid;

        return FFTBandType.High;
    }
}
