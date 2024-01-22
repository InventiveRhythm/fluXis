using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.Sprites;

public class FontAwesome6
{
    private static IconUsage get(int icon) => new((char)icon, "FontAwesome6");

    public static IconUsage GetSolid(int icon) => get(icon).With(weight: "Solid");
    public static IconUsage GetRegular(int icon) => get(icon).With(weight: "Regular");
    public static IconUsage GetLight(int icon) => get(icon).With(weight: "Light");
}
