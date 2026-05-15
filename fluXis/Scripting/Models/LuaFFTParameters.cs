using fluXis.Audio.FFT.Structures.Processor;
using NLua;

namespace fluXis.Scripting.Models;

/// <summary>
/// Wrapper class for FFTParameters' presets. The actual definitions are in <see cref="FFTParameters"/>
/// </summary>
public class LuaFFTParameters : ILuaModel
{
    [LuaMember(Name = "Default")]
    public FFTParameters Default => FFTParameters.Default;

    [LuaMember(Name = "Reactive")]
    public FFTParameters Reactive => FFTParameters.Reactive;

    [LuaMember(Name = "Smooth")]
    public FFTParameters Smooth => FFTParameters.Smooth;
}
