namespace fluXis.Game.Utils;

public class PathUtils
{
    public static string HashToPath(string hash)
    {
        return hash.Substring(0, 1) + "/" + hash.Substring(0, 2) + "/" + hash;
    }
}
