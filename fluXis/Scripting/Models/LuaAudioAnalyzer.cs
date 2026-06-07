using fluXis.Audio.FFT;
using fluXis.Audio.FFT.Structures.Processor;
using fluXis.Scripting.Attributes;
using fluXis.Utils.Extensions;
using NLua;

namespace fluXis.Scripting.Models;

[LuaDefinition("audio", Name = "AudioAnalyzer", Public = true)]
public class LuaAudioAnalyzer : ILuaModel
{
    private AudioAnalyzer analyzer { get; }
    private Lua lua { get; }

    public LuaAudioAnalyzer(AudioAnalyzer analyzer, Lua lua = null)
    {
        this.analyzer = analyzer;
        this.lua = lua;
    }

    [LuaMember(Name = "AmplitudesInRange")]
    public LuaTable GetAmplitudesInRange(
        double startTime,
        double endTime,
        double interval,
        int? amplitudeCount = AudioAnalyzer.FFT_BINS,
        FFTParameters? parameters = null
    )
        => analyzer.GetAmplitudes(
            (int)startTime,
            (int)endTime,
            (int)interval,
            amplitudeCount ?? AudioAnalyzer.FFT_BINS,
            (parameters is null) ? null : new FFTProcessor(parameters)
        ).ToLuaTable(lua);
}
