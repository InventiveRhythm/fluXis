using System.Diagnostics;
using osu.Framework;

namespace fluXis.Game.Utils;

public static class PathUtils
{
    public static string HashToPath(string hash)
    {
        if (hash.Length < 3) return hash;

        return hash[..1] + "/" + hash[..2] + "/" + hash;
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
}
