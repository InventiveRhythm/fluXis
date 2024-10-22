using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Development;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Threading;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Utils.Extensions;

public static class DrawableExtensions
{
    public static T CacheAsAndReturn<T>(this DependencyContainer deps, T draw)
        where T : class
    {
        deps.CacheAs(draw);
        return draw;
    }

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

    public static void Rainbow(this Drawable drawable)
    {
        const float len = 800;

        var colors = new[]
        {
            Colour4.FromHSL(0, 1, .66f),
            Colour4.FromHSL(30 / 360f, 1, .66f),
            Colour4.FromHSL(60 / 360f, 1, .66f),
            Colour4.FromHSL(90 / 360f, 1, .66f),
            Colour4.FromHSL(120 / 360f, 1, .66f),
            Colour4.FromHSL(150 / 360f, 1, .66f),
            Colour4.FromHSL(180 / 360f, 1, .66f),
            Colour4.FromHSL(210 / 360f, 1, .66f),
            Colour4.FromHSL(240 / 360f, 1, .66f),
            Colour4.FromHSL(270 / 360f, 1, .66f),
            Colour4.FromHSL(300 / 360f, 1, .66f),
            Colour4.FromHSL(330 / 360f, 1, .66f),
        };

        drawable.Colour = colors[0];

        var seq = drawable.FadeColour(colors[1], len);

        for (var i = 2; i < colors.Length + 2; i++)
        {
            var col = colors[i % colors.Length];
            seq.Then().FadeColour(col, len);
        }

        seq.Loop();
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

    public static Anchor OppositeHorizontal(this Anchor anchor)
    {
        if (anchor.HasFlagFast(Anchor.x0) || anchor.HasFlagFast(Anchor.x2))
            anchor ^= Anchor.x0 | Anchor.x2;

        return anchor;
    }
}
