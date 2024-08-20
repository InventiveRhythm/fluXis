using System;
using System.Collections.Generic;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Threading;
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

    public static void Vibrate(this Drawable target, double duration, float magnitude)
    {
        target.MoveToX(magnitude, duration / 2, Easing.OutSine)
              .Then().MoveToX(-magnitude, duration, Easing.InOutSine)
              .Then().MoveToX(magnitude, duration, Easing.InOutSine)
              .Then().MoveToX(-magnitude, duration, Easing.InOutSine)
              .Then().MoveToX(0, duration / 2, Easing.InSine);
    }

    public static void RunOnUpdate(this Drawable _, Scheduler scheduler, Action action)
    {
        if (ThreadSafety.IsUpdateThread)
            action.Invoke();
        else
            scheduler.Add(action.Invoke);
    }

    public static void ScheduleIfNeeded(this Scheduler scheduler, Action action)
    {
        if (ThreadSafety.IsUpdateThread)
            action.Invoke();
        else
            scheduler.Add(action.Invoke);
    }

    public static void ScheduleOnceIfNeeded(this Scheduler scheduler, Action action)
    {
        if (ThreadSafety.IsUpdateThread)
            action.Invoke();
        else
            scheduler.AddOnce(action.Invoke);
    }

    public static TransformSequence<T> BorderTo<T>(this T drawable, float newBorder, float duration = 0f, Easing ease = Easing.None)
        where T : class, ITransformable
        => drawable.TransformTo(nameof(Container.BorderThickness), newBorder, duration, ease);

    public static TransformSequence<T> BorderTo<T>(this TransformSequence<T> seq, float newBorder, float duration = 0f, Easing ease = Easing.None)
        where T : class, ITransformable
        => seq.Append(s => s.TransformTo(nameof(Container.BorderThickness), newBorder, duration, ease));
}
