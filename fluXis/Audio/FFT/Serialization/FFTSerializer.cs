using System;
using System.Linq;
using System.Text;
using fluXis.Audio.FFT.Structures.Cache;
using K4os.Compression.LZ4;

namespace fluXis.Audio.FFT.Serialization;

public class FFTSerializer
{
    private readonly FFTCache cache;
    private readonly StringBuilder sb;
    private long totalDataSize;

    private const LZ4Level compression_level = LZ4Level.L12_MAX;

    public FFTSerializer(FFTCache cache)
    {
        this.cache = cache;

        sb = new StringBuilder();
        writeInfoHeader();
    }

    private void writeInfoHeader()
    {
        sb.AppendLine("[Info]");
        sb.AppendLine($"version: 1"); // TODO: properly implement versioning later if changed
        sb.AppendLine($"hash: {cache.Hash}");
        sb.AppendLine($"bins: {cache.BinCount}");
        sb.AppendLine($"frame_size: {cache.FrameLen}");
        sb.AppendLine($"resolution: {cache.Resolution}");
        sb.AppendLine($"lz4_level: {(int)compression_level}");
        sb.AppendLine($"uncompressed_size: {totalDataSize = cache.Ranges.Sum(r => r.DataLength)}");
        sb.Append("ranges: {");

        long offset = 0;
        bool first = true;

        foreach (var range in cache.Ranges)
        {
            long end = offset + range.DataLength;
            if (!first) sb.Append(',');
            sb.Append($"[{range.TStart},{range.TEnd}]");
            offset = end;
            first = false;
        }

        sb.AppendLine("}");
    }

    public byte[] Serialize()
    {
        sb.AppendLine("[Data]");

        byte[] header = Encoding.UTF8.GetBytes(sb.ToString());

        byte[] data = new byte[totalDataSize];

        int pos = 0;

        foreach (var range in cache.Ranges)
        {
            range.Data.AsSpan(0, (int)range.DataLength).CopyTo(data.AsSpan(pos));
            pos += (int)range.DataLength;
        }

        byte[] dataCompressed = new byte[LZ4Codec.MaximumOutputSize((int)totalDataSize)];
        int compressedDataSize = LZ4Codec.Encode(data, dataCompressed, compression_level);

        byte[] result = new byte[header.Length + compressedDataSize];

        header.CopyTo(result, 0);
        dataCompressed.AsSpan(0, compressedDataSize).CopyTo(result.AsSpan(header.Length));

        return result;
    }
}
