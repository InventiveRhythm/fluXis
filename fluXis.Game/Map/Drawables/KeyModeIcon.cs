using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Map.Drawables;

public partial class KeyModeIcon : Container
{
    public int KeyMode { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Colour = FluXisColors.GetKeyColor(KeyMode);

        switch (KeyMode)
        {
            case 4:
                AddRange(new[]
                {
                    getCircle(-6, -6),
                    getCircle(-6, 6),
                    getCircle(6, -6),
                    getCircle(6, 6)
                });
                break;

            case 5:
                AddRange(new[]
                {
                    getCircle(-9, -9),
                    getCircle(9, -9),
                    getCircle(0, 0),
                    getCircle(-9, 9),
                    getCircle(9, 9)
                });
                break;

            case 6:
                AddRange(new[]
                {
                    getCircle(-6, -11),
                    getCircle(6, -11),
                    getCircle(-12, 0),
                    getCircle(12, 0),
                    getCircle(6, 11),
                    getCircle(-6, 11)
                });
                break;

            case 7:
                AddRange(new[]
                {
                    getCircle(-6, -11),
                    getCircle(6, -11),
                    getCircle(-12, 0),
                    getCircle(0, 0),
                    getCircle(12, 0),
                    getCircle(6, 11),
                    getCircle(-6, 11)
                });
                break;

            case 8:
                AddRange(new[]
                {
                    getCircle(0, -14),
                    getCircle(-10, -10),
                    getCircle(10, -10),
                    getCircle(-14, 0),
                    getCircle(14, 0),
                    getCircle(-10, 10),
                    getCircle(10, 10),
                    getCircle(0, 14)
                });
                break;

            default:
                Add(new FluXisSpriteText
                {
                    Text = $"{KeyMode}K"
                });
                break;
        }
    }

    private Drawable getCircle(float x, float y)
    {
        return new Circle
        {
            Size = new Vector2(10),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Position = new Vector2(x, y)
        };
    }
}
