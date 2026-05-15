using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio.FFT.Structures.Data;
using osu.Framework.Utils;

namespace fluXis.Audio.FFT.Structures.Cache;

public class FFTCache : IDisposable
{
    private readonly List<FFTRange> ranges = new();
    public IReadOnlyList<FFTRange> Ranges => ranges;

    public int BinCount = AudioAnalyzer.FFT_BINS;
    public uint FrameLen = AudioAnalyzer.FRAME_SIZE;
    public uint Resolution = AudioAnalyzer.RESOLUTION;

    public required string Hash;

    public FFTFrame[] GetFrames(int tStart, int tEnd, int interval)
    {
        int outputFrames = (tEnd - tStart) / interval + 1;
        var result = new FFTFrame[outputFrames];

        for (int i = 0; i < outputFrames; i++)
        {
            int t = tStart + i * interval;

            int tLow = (t / (int)Resolution) * (int)Resolution;
            int tHigh = tLow + (int)Resolution;
            float w = (t - tLow) / (float)Resolution;

            ReadOnlySpan<byte> frameLow = getRawFrame(tLow);
            ReadOnlySpan<byte> frameHigh = getRawFrame(tHigh);

            float[] bins = new float[BinCount];
            for (int b = 0; b < BinCount; b++)
                bins[b] = (float)Interpolation.Lerp(frameLow[b] / 255f, frameHigh[b] / 255f, w);

            result[i] = new FFTFrame(
                bins,
                new FFTBands(
                    low: (float)Interpolation.Lerp(frameLow[BinCount + 0] / 255f, frameHigh[BinCount + 0] / 255f, w),
                    mid: (float)Interpolation.Lerp(frameLow[BinCount + 1] / 255f, frameHigh[BinCount + 1] / 255f, w),
                    high: (float)Interpolation.Lerp(frameLow[BinCount + 2] / 255f, frameHigh[BinCount + 2] / 255f, w),
                    total: (float)Interpolation.Lerp(frameLow[BinCount + 3] / 255f, frameHigh[BinCount + 3] / 255f, w)
                ));
        }

        return result;
    }

    private ReadOnlySpan<byte> getRawFrame(int t)
    {
        if (t < 0)
            return new byte[FrameLen];

        var range = ranges.Find(r => r.TStart <= (uint)t && r.TEnd > (uint)t);
        if (range == null) return new byte[FrameLen];

        int frame = (t - (int)range.TStart) / (int)Resolution;
        int offset = frame * (int)FrameLen;

        if (offset + FrameLen > range.DataLength)
            return new byte[FrameLen];

        return range.Data.AsSpan(offset, (int)FrameLen);
    }

    public void Add(FFTRange range)
    {
        ranges.Add(range);
        ranges.Sort();
        mergeOverlapping();
    }

    /// <summary>
    /// NEVER call this unless you know what you are doing!
    /// </summary>
    public void AddUnchecked(FFTRange range)
    {
        ranges.Add(range);
    }

    public void Remove(FFTRange range)
    {
        ranges.Remove(range);
        range?.Dispose();
    }

    public bool ContainsRange(uint tStart, uint tEnd, out List<(uint Start, uint End)> missing)
    {
        missing = new List<(uint, uint)>();
        uint cursor = tStart;

        foreach (var range in ranges.Where(range => range.TEnd >= cursor)
                                    .TakeWhile(range => range.TStart <= tEnd))
        {
            if (range.TStart > cursor)
                missing.Add((cursor, range.TStart));

            cursor = Math.Max(cursor, range.TEnd);
        }

        if (cursor < tEnd)
            missing.Add((cursor, tEnd));

        return missing.Count == 0;
    }

    public bool ContainsAndAllocate(uint tStart, uint tEnd, out List<FFTRange> missing)
    {
        missing = [];
        bool fullyContained = ContainsRange(tStart, tEnd, out var gaps);

        foreach (var (start, end) in gaps)
        {
            var range = allocate(start, end);
            missing.Add(range);
            Add(range);
        }

        return fullyContained;
    }

    private FFTRange allocate(uint tStart, uint tEnd)
    {
        // align boundaries to resolution to prevent inconsistent frame counts
        uint alignedStart = (tStart / Resolution) * Resolution;
        uint alignedEnd = (uint)Math.Ceiling((double)tEnd / Resolution) * Resolution;

        var range = new FFTRange
        {
            TStart = alignedStart,
            TEnd = alignedEnd,
        };
        range.Allocate();
        return range;
    }

    private void mergeOverlapping()
    {
        int i = 0;

        while (i < ranges.Count - 1)
        {
            var current = ranges[i];
            var next = ranges[i + 1];

            if (current.TEnd >= next.TStart)
            {
                ranges[i] = merge(current, next);
                ranges.RemoveAt(i + 1);
            }
            else
            {
                i++;
            }
        }
    }

    private FFTRange merge(FFTRange a, FFTRange b)
    {
        var merged = new FFTRange
        {
            TStart = Math.Min(a.TStart, b.TStart),
            TEnd = Math.Max(a.TEnd, b.TEnd)
        };

        merged.Allocate();

        int aOffset = (int)((a.TStart - merged.TStart) / Resolution) * (int)FrameLen;
        Buffer.BlockCopy(a.Data, 0, merged.Data, aOffset, (int)a.DataLength);

        int bOffset = (int)((b.TStart - merged.TStart) / Resolution) * (int)FrameLen;
        Buffer.BlockCopy(b.Data, 0, merged.Data, bOffset, (int)b.DataLength);

        a.Dispose();
        b.Dispose();
        return merged;
    }

    public void Dispose()
    {
        foreach (var range in ranges)
            range.Dispose();
        ranges.Clear();

        GC.SuppressFinalize(this);
    }
}
