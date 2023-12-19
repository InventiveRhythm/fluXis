using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;
using osuTK;

namespace fluXis.Game.Graphics;

public static class FluXisStyles
{
    public static EdgeEffectParameters ShadowSmall => createShadow(5, 2);
    public static EdgeEffectParameters ShadowSmallNoOffset => createShadow(5, 0);

    public static EdgeEffectParameters ShadowMedium => createShadow(10, 2);
    public static EdgeEffectParameters ShadowMediumNoOffset => createShadow(10, 0);

    public static EdgeEffectParameters ShadowLarge => createShadow(20, 2);
    public static EdgeEffectParameters ShadowLargeNoOffset => createShadow(20, 0);

    public static EdgeEffectParameters SnowShadow => new()
    {
        Type = EdgeEffectType.Shadow,
        Colour = Colour4.White,
        Radius = 10
    };

    private static EdgeEffectParameters createShadow(int radius, float offset) => new()
    {
        Type = EdgeEffectType.Shadow,
        Colour = Colour4.Black.Opacity(.25f),
        Radius = radius,
        Offset = new Vector2(0, offset)
    };
}
