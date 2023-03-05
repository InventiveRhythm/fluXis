namespace fluXis.Game.Utils;

public class PathUtils
{
    public static string HashToPath(string hash)
    {
        return hash[..1] + "/" + hash[..2] + "/" + hash;
    }
}
