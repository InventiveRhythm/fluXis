using System;
using System.Buffers;

namespace fluXis.Audio.FFT.Structures.Cache;

public class FFTRange : IDisposable, IComparable<FFTRange>
{
    public uint TStart;
    public uint TEnd;

    public byte[] Data { get; private set; }
    public long DataLength { get; private set; }

    public long FrameCount => (TEnd - TStart) / AudioAnalyzer.RESOLUTION;

    public void Allocate()
    {
        long size = FrameCount * AudioAnalyzer.FRAME_SIZE;
        Data = ArrayPool<byte>.Shared.Rent((int)size);
        DataLength = size;
    }

    public int CompareTo(FFTRange other = null)
    {
        return other is null ? 1 : TStart.CompareTo(other.TStart);
    }

    public void Dispose()
    {
        if (Data == null) return;

        ArrayPool<byte>.Shared.Return(Data);
        Data = null;
    }
}
