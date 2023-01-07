using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics
{
    public class TicTac : CircularContainer
    {
        public TicTac(float size)
        {
            Size = new Vector2(size / 2, size);
            Masking = true;

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White,
            };
        }
    }
}
