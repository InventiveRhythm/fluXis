using System.Diagnostics;
using System.IO;
using osu.Framework;

namespace fluXis.Game.Utils;

public static class PathUtils
{
    public static string HashToPath(string hash)
    {
        if (hash.Length < 3) return hash;

        return hash[..1] + "/" + hash[..2] + "/" + hash;
    }

    public static string RemoveAllInvalidPathCharacters(string path)
    {
        path = string.Concat(path.Split(Path.GetInvalidPathChars()));
        return string.Concat(path.Split(Path.GetInvalidFileNameChars()));
    }

    public static void OpenFolder(string path)
    {
        switch (RuntimeInfo.OS)
        {
            case RuntimeInfo.Platform.Windows:
                Process.Start("explorer.exe", path);
                break;

            case RuntimeInfo.Platform.macOS:
                Process.Start("open", path);
                break;

            case RuntimeInfo.Platform.Linux:
                Process.Start("xdg-open", path);
                break;
        }
    }

    public static void ShowFile(string path)
    {
        switch (RuntimeInfo.OS)
        {
            case RuntimeInfo.Platform.Windows:
                Process.Start("explorer.exe", "/select, " + path);
                break;

            case RuntimeInfo.Platform.macOS:
                Process.Start("open", "-R " + path);
                break;

            case RuntimeInfo.Platform.Linux:
                Process.Start("xdg-open", path);
                break;
        }
    }
}
