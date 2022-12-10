using osu.Framework.Graphics;

namespace fluXis.Game.Graphics
{
    public class MoreBlendModes
    {
        public static BlendingParameters Erase => new BlendingParameters
        {
            Source = BlendingType.Zero,
            Destination = BlendingType.OneMinusSrcAlpha,
            RGBEquation = BlendingEquation.ReverseSubtract,
            AlphaEquation = BlendingEquation.ReverseSubtract,
        };
    }
}
