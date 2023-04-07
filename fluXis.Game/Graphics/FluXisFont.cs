using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics;

public class FluXisFont
{
    public static FontUsage Default(float size = 20, bool fixedWidth = false) => new("Renogare", size, fixedWidth: fixedWidth);
}
