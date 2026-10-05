using System.IO;
using osu.Framework.Platform;

namespace fluXis.Utils.Extensions;

public static class StorageExtensions
{
    public static string ReadAllText(this Storage storage, string path)
    {
        using var reader = new StreamReader(storage.GetStream(path));
        return reader.ReadToEnd();
    }

    public static void WriteAllText(this Storage storage, string path, string content)
    {
        using var stream = storage.GetStream(path, FileAccess.ReadWrite);

        // clear content
        stream.SetLength(0);
        stream.Position = 0;

        using var writer = new StreamWriter(stream);
        writer.Write(content);
    }
}
