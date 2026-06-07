using fluXis.Scripting.Attributes;
using NLua;

namespace fluXis.Audio.FFT.Structures.Processor;

/// <summary>
/// Parameters to be used for the <see cref="FFTProcessor"/>.
/// </summary>
public record struct FFTParameters
{
    /// <summary>
    /// How fast the amplitude values rise.
    /// <para>Lower values (e.g. 0.50): Slower rise and looks slightly delayed.</para>
    /// <para>Higher values (e.g. 0.95): Instant and snappy that closely resembles the raw audio.</para>
    /// </summary>
    [LuaMember(Name = "attack")]
    public float Attack { get; set; }

    /// <summary>
    /// How fast the lows (Bass) fall back down after a sound stops.
    /// <para>Lower values (e.g. 0.05): Slow decay, causing bass peaks to linger.</para>
    /// <para>Higher values (e.g. 0.40): Fast decay, making kicks and bass end sharply and cleanly.</para>
    /// </summary>
    [LuaMember(Name = "releaseLow")]
    public float ReleaseLow { get; set; }

    /// <summary>
    /// How fast the highs (Treble) fall back down after a sound stops.
    /// <para>Lower values (e.g. 0.20): Smoothes out rapidly fluctuating high frequencies (like cymbals).</para>
    /// <para>Higher values (e.g. 0.80): Fast and highly reactive. Can cause jitter if too high.</para>
    /// </summary>
    [LuaMember(Name = "releaseHigh")]
    public float ReleaseHigh { get; set; }

    /// <summary>
    /// Exponentiates the final amplitudes (value^gamma). This is essentially the contrast.
    /// <para>Lower values (&lt; 1.0): Bows the curve upwards. Boosts quiet noise to be more visible.</para>
    /// <para>Higher values (&gt; 2.0): Bows the curve downwards. Squashes quiet noise and isolates only the loudest peaks.</para>
    /// </summary>
    [LuaMember(Name = "gamma")]
    public float Gamma { get; set; }

    /// <summary>
    /// How many adjacent bins to average AKA. The smoothness of the final curves.
    /// <para>Lower values (1): No smoothing. Represents exact frequencies but can produce jittery amplitudes.</para>
    /// <para>Higher values (&gt; 5): Blends the adjacent curves into a smoother overall shape.</para>
    /// </summary>
    [LuaMember(Name = "spatialWindowSize")]
    public int SpatialWindowSize { get; set; }

    /// <summary>
    /// The point on the frequency spectrum where the "Low (Bass)" ends and "Mid" begins. Spectrum range: [0.0 to 1.0].
    /// </summary>
    [LuaMember(Name = "bassCutoff")]
    public float BassCutoff { get; set; }

    /// <summary>
    /// The point on the frequency spectrum where the "Mid" band ends and "High (Treble)" begins. Spectrum range: [0.0 to 1.0].
    /// </summary>
    [LuaMember(Name = "midCutoff")]
    public float MidCutoff { get; set; }

    /// <summary>
    /// How much to boost the bass frequency band.
    /// <para>Lower values (&lt; 1.0): Lowers the intensity of kicks and bass.</para>
    /// <para>Higher values (&gt; 1.0): Increases the intensity of kicks and bass.</para>
    /// </summary>
    [LuaMember(Name = "bassMultiplier")]
    public float BassMultiplier { get; set; }

    /// <summary>
    /// How much to boost the mid frequency band.
    /// <para>Lower values (&lt; 1.0): Lowers the intensity of vocals, synths, and guitars.</para>
    /// <para>Higher values (&gt; 1.0): Increases the intensity of vocals, synths, and guitars.</para>
    /// </summary>
    [LuaMember(Name = "midMultiplier")]
    public float MidMultiplier { get; set; }

    /// <summary>
    /// How much to boost the high frequency band.
    /// <para>Lower values (&lt; 1.0): Lowers the intensity of hi-hats, cymbals, and treble.</para>
    /// <para>Higher values (&gt; 1.0): Increases the intensity of hi-hats, cymbals, and treble.</para>
    /// </summary>
    [LuaMember(Name = "highMultiplier")]
    public float HighMultiplier { get; set; }

    /// <summary>
    /// The minimum peak volume for the bass frequency band. Prevents auto normalization from amplifying pure silence.
    /// <para>Lower values (e.g. 0.1): Quiet bass gets scaled up aggressively.</para>
    /// <para>Higher values (e.g. 0.4): Only loud bass hits register strongly.</para>
    /// </summary>
    [LuaMember(Name = "baseFloor")]
    public float BassFloor { get; set; }

    /// <summary>
    /// The minimum peak volume for the mid frequency band.
    /// <para>Lower values (e.g. 0.1): Quiet mids get scaled up aggressively.</para>
    /// <para>Higher values (e.g. 0.4): Only loud mids register strongly.</para>
    /// </summary>
    [LuaMember(Name = "midFloor")]
    public float MidFloor { get; set; }

    /// <summary>
    /// The minimum peak volume for the high frequency band.
    /// <para>Lower values (e.g. 0.1): Quiet highs get scaled up aggressively.</para>
    /// <para>Higher values (e.g. 0.4): Only loud highs register strongly.</para>
    /// </summary>
    [LuaMember(Name = "highFloor")]
    public float HighFloor { get; set; }

    /// <summary>
    /// How fast the dynamic auto-leveling normalizes the current volume of the track.
    /// <para>Lower values (e.g. 0.01): Slow adaptation. Preserves contrast between quiet and loud sections.</para>
    /// <para>Higher values (e.g. 0.15): Fast adaptation. Keeps output intensity roughly consistent regardless of song volume.</para>
    /// </summary>
    [LuaMember(Name = "maxAdaptationRate")]
    public float MaxAdaptationRate { get; set; }

    [LuaGlobal(Name = "Default")]
    public static FFTParameters Default => new()
    {
        Attack = 0.95f,
        ReleaseLow = 0.12f,
        ReleaseHigh = 0.75f,
        Gamma = 2.2f,
        SpatialWindowSize = 3,

        BassCutoff = 0.25f,
        MidCutoff = 0.60f,

        BassMultiplier = 1.0f,
        MidMultiplier = 1.0f,
        HighMultiplier = 1.0f,

        BassFloor = 0.3f,
        MidFloor = 0.2f,
        HighFloor = 0.1f,

        MaxAdaptationRate = 0.05f
    };

    [LuaGlobal(Name = "Reactive")]
    public static FFTParameters Reactive => new()
    {
        Attack = 0.80f,
        ReleaseLow = 0.05f,
        ReleaseHigh = 0.40f,
        Gamma = 1.8f,
        SpatialWindowSize = 1,

        BassCutoff = 0.25f,
        MidCutoff = 0.60f,

        BassMultiplier = 1.3f,
        MidMultiplier = 1.1f,
        HighMultiplier = 1.0f,

        BassFloor = 0.15f,
        MidFloor = 0.10f,
        HighFloor = 0.05f,

        MaxAdaptationRate = 0.15f
    };

    [LuaGlobal(Name = "Smooth")]
    public static FFTParameters Smooth => new()
    {
        Attack = 0.98f,
        ReleaseLow = 0.20f,
        ReleaseHigh = 0.85f,
        Gamma = 2.5f,
        SpatialWindowSize = 5,

        BassCutoff = 0.25f,
        MidCutoff = 0.60f,

        BassMultiplier = 1.0f,
        MidMultiplier = 1.1f,
        HighMultiplier = 0.8f,

        BassFloor = 0.4f,
        MidFloor = 0.3f,
        HighFloor = 0.2f,

        MaxAdaptationRate = 0.01f
    };
}
