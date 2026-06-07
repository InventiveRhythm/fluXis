using K4os.Compression.LZ4;

namespace fluXis.Audio.FFT.Structures;

/// <summary>
/// Only used for deserialization
/// </summary>
public struct FFTInfo
{
    public string Hash { get; set; }
    public int BinCount { get; set; }
    public uint FrameLen { get; set; }
    public uint Resolution { get; set; }
    public LZ4Level Lz4Level { get; set; }
    public long UncompressedSize { get; set; }
    public (uint tStart, uint tEnd)[] Ranges { get; set; }
}
