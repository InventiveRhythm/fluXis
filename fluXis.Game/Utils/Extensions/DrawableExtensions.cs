using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Utils.Extensions;

public static class DrawableExtensions
{
    public static void Shake(this Drawable target, double shakeDuration, float shakeMagnitude)
    {
        var rngPositions = new List<Vector2>();

        for (var i = 0; i < 4; i++)
        {
            var x = RNG.NextSingle(-shakeMagnitude, shakeMagnitude);
            var y = RNG.NextSingle(-shakeMagnitude, shakeMagnitude);
            rngPositions.Add(new Vector2(x, y));
        }

        var sequence = target.MoveTo(rngPositions[0], shakeDuration / 5, Easing.OutSine).Then();

        for (var i = 0; i < 3; i++)
            sequence = sequence.MoveTo(rngPositions[i + 1], shakeDuration / 5, Easing.InOutSine).Then();

        sequence.MoveTo(Vector2.Zero, shakeDuration / 5, Easing.InSine);
    }
}
