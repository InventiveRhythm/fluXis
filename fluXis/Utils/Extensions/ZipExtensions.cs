using System.IO;
using System.IO.Compression;
using osu.Framework.Extensions;

namespace fluXis.Utils.Extensions;

public static class ZipExtensions
{
    public static byte[] ReadAllBytes(this ZipArchiveEntry entry)
    {
        using Stream entryStream = entry.Open();

        return entryStream.ReadAllRemainingBytesToArray();
    }
}
