using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Graphics.Drawables;

public partial class TicTac : CircularContainer
{
    private float size;

    public float TicTacSize
    {
        get => size;
        set
        {
            Size = new Vector2(value / 2, value);
            size = value;
        }
    }

    public TicTac(float size)
    {
        TicTacSize = size;
        Masking = true;

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Theme.Text,
        };
    }
}
