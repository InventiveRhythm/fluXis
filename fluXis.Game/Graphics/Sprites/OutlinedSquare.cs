using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;

namespace fluXis.Game.Graphics.Sprites;

public partial class OutlinedSquare : Container
{
    public OutlinedSquare()
    {
        BorderColour = Colour4.White;
        BorderThickness = 20;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Transparent
        };
    }
}

public static class OutlinedSquareExtensions
{
    public static TransformSequence<OutlinedSquare> BorderTo(this OutlinedSquare square, float thickness, double duration, Easing easing)
        => square.TransformTo(nameof(square.BorderThickness), thickness, duration, easing);

    public static TransformSequence<OutlinedSquare> BorderTo(this TransformSequence<OutlinedSquare> sequence, float thickness, double duration, Easing easing)
        => sequence.Append(o => o.BorderTo(thickness, duration, easing));
}
