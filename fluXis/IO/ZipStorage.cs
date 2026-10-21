using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Platform;

namespace fluXis.IO;

#nullable enable

public class ZipStorage : Storage, IDisposable, IAsyncDisposable
{
    private readonly ZipArchive archive;

    public ZipStorage(ZipArchive archive)
        : base("/")
    {
        this.archive = archive;
    }

    private string normalize(string path) => path.Replace('\\', '/');

    private ZipArchiveEntry? find(string path)
    {
        path = normalize(path);

        foreach (var entry in archive.Entries)
        {
            if (normalize(entry.FullName) == path)
                return entry;
        }

        return null;
    }

    public override string GetFullPath(string path, bool createIfNotExisting = false) => Path.Combine(BasePath, path);

    public override bool Exists(string path) => find(path) != null;

    public override bool ExistsDirectory(string path) => true;

    public override void DeleteDirectory(string path)
        => throw new InvalidOperationException("Cannot modify a zip file through ZipStorage.");

    public override void Delete(string path)
        => throw new InvalidOperationException("Cannot modify a zip file through ZipStorage.");

    public override IEnumerable<string> GetDirectories(string path)
        => throw new InvalidOperationException(); // cba

    public override IEnumerable<string> GetFiles(string path, string pattern = "*")
    {
        path = normalize(path);
        return archive.Entries.Select(x => normalize(x.FullName)).Where(x => x.StartsWith(path));
    }

    public override void Move(string from, string to)
        => throw new InvalidOperationException("Cannot modify a zip file through ZipStorage.");

    public override Stream GetStream(string path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.OpenOrCreate)
    {
        if (access != FileAccess.Read)
            throw new InvalidOperationException("Cannot modify a zip file through ZipStorage.");

        var match = find(path) ?? throw new FileNotFoundException();
        return match.Open();
    }

    public override bool OpenFileExternally(string filename) => false;
    public override bool PresentFileExternally(string filename) => false;

    public void Dispose()
    {
        archive.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await archive.DisposeAsync();
    }
}
