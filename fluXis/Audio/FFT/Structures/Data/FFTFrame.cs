using System.Linq;
using NLua;

namespace fluXis.Audio.FFT.Structures.Data;

public readonly struct FFTFrame
{
    [LuaMember(Name = "amplitudes")]
    public readonly float[] Amplitudes;

    [LuaMember(Name = "bands")]
    public readonly FFTBands Bands;

    public FFTFrame(float[] amplitudes, FFTBands bands)
    {
        Amplitudes = amplitudes;
        Bands = bands;
    }

    [LuaMember(Name = "IsSilent")]
    public bool IsSilent(double threshold = 0.03)
        => Amplitudes.Length == 0 || Amplitudes.Max() < threshold;

    [LuaMember(Name = "DetectBeat")]
    public bool DetectBeat(double threshold = 0.7)
        => Amplitudes.Length > 0 && Amplitudes.Max() > threshold;

    [LuaMember(Name = "GetPeakAmplitude")]
    public double GetPeakAmplitude()
        => Amplitudes.Length > 0 ? Amplitudes.Max() : 0;

    [LuaMember(Name = "GetAverageAmplitude")]
    public double GetAverageAmplitude()
        => Amplitudes.Length > 0 ? Amplitudes.Average() : 0;

    [LuaMember(Name = "GetPeakFrequencyBin")]
    public int GetPeakFrequencyBin()
    {
        if (Amplitudes.Length == 0) return -1;

        var max = Amplitudes.Max();
        return System.Array.IndexOf(Amplitudes, max);
    }
}
