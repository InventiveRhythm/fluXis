using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using fluXis.Audio.FFT.Structures;
using fluXis.Audio.FFT.Structures.Cache;
using K4os.Compression.LZ4;

namespace fluXis.Audio.FFT.Serialization;

public class FFTDeserializer
{
    private FFTInfo info;
    private readonly byte[] input;
    private int dataStart;

    public FFTDeserializer(byte[] input)
    {
        this.input = input;
        parseInfo();
    }

    private void parseInfo()
    {
        byte[] dataMarker = Encoding.UTF8.GetBytes("[Data]");
        int markerPos = findSequence(input, dataMarker);

        if (markerPos < 0)
            throw new InvalidDataException("Missing [Data] section.");

        dataStart = markerPos + dataMarker.Length;
        if (dataStart < input.Length && input[dataStart] == '\r') dataStart++;
        if (dataStart < input.Length && input[dataStart] == '\n') dataStart++;

        string header = Encoding.UTF8.GetString(input, 0, markerPos);

        string hash = null;
        int bins = AudioAnalyzer.FFT_BINS;
        uint frameLen = AudioAnalyzer.FRAME_SIZE;
        uint resolution = AudioAnalyzer.RESOLUTION;
        LZ4Level lz4Level = LZ4Level.L12_MAX;
        long uncompressedSize = AudioAnalyzer.FRAME_SIZE;
        var ranges = new List<(uint tStart, uint tEnd)>();

        bool inInfo = false;

        foreach (string rawLine in header.Split('\n'))
        {
            string line = rawLine.TrimEnd('\r');

            if (line == "[Info]")
            {
                inInfo = true;
                continue;
            }

            if (!inInfo) continue;

            int colonIdx = line.IndexOf(':');
            if (colonIdx < 0) continue;

            string key = line[..colonIdx].Trim();
            string value = line[(colonIdx + 1)..].Trim();

            switch (key)
            {
                case "hash": hash = value; break;

                case "bins": bins = int.Parse(value); break;

                case "frame_size": frameLen = uint.Parse(value); break;

                case "resolution": resolution = uint.Parse(value); break;

                case "lz4_level": lz4Level = (LZ4Level)(int.Parse(value)); break;

                case "uncompressed_size": uncompressedSize = long.Parse(value); break;

                case "ranges": ranges = parseRanges(value); break;
            }
        }

        info = new FFTInfo
        {
            Hash = hash,
            BinCount = bins,
            FrameLen = frameLen,
            Resolution = resolution,
            Lz4Level = lz4Level,
            UncompressedSize = uncompressedSize,
            Ranges = ranges.ToArray()
        };
    }

    private static List<(uint tStart, uint tEnd)> parseRanges(string value)
    {
        var result = new List<(uint, uint)>();
        string inner = value.Trim('{', '}');

        if (string.IsNullOrWhiteSpace(inner))
            return result;

        int i = 0;

        while (i < inner.Length)
        {
            if (inner[i] != '[')
            {
                i++;
                continue;
            }

            int close = inner.IndexOf(']', i);
            string[] parts = inner[(i + 1)..close].Split(',');
            result.Add((uint.Parse(parts[0]), uint.Parse(parts[1])));
            i = close + 2; // skip "],"
        }

        return result;
    }

    private static int findSequence(byte[] data, byte[] sequence)
    {
        for (int i = 0; i <= data.Length - sequence.Length; i++)
        {
            bool found = !sequence.Where((t, j) => data[i + j] != t).Any();

            if (found) return i;
        }

        return -1;
    }

    public FFTInfo GetInfo() => info;

    public FFTCache Deserialize()
    {
        var cache = new FFTCache
        {
            Hash = info.Hash,
            BinCount = info.BinCount,
            FrameLen = info.FrameLen,
            Resolution = info.Resolution
        };

        byte[] decompressed = new byte[info.UncompressedSize];
        LZ4Codec.Decode(input.AsSpan(dataStart), decompressed);

        int pos = 0;

        foreach (var (tStart, tEnd) in info.Ranges)
        {
            var range = new FFTRange { TStart = tStart, TEnd = tEnd };
            range.Allocate();

            decompressed.AsSpan(pos, (int)range.DataLength).CopyTo(range.Data);
            pos += (int)range.DataLength;

            cache.AddUnchecked(range);
        }

        return cache;
    }
}
