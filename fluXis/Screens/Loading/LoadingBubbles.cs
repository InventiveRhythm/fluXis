using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Loading;

public partial class LoadingBubbles : CompositeDrawable
{
    private const float min_alpha = .5f;
    private const float max_alpha = 1;
    private const double min_fade_time = 1000;
    private const double max_fade_time = 3000;
    private const double min_scale_time = 1000;
    private const double max_scale_time = 8000;
    private const float min_start_size = 10;
    private const float max_start_size = 600;
    private const float min_extra_size = 20;
    private const float max_extra_size = 200;
    private const double spawn_cooldown = 200;
    private readonly Colour4 particleColor = Theme.Highlight;

    private double lastSpawn = -spawn_cooldown;

    public LoadingBubbles()
    {
        RelativeSizeAxes = Axes.Both;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.Current - lastSpawn < spawn_cooldown)
            return;

        var alpha = RNG.NextSingle(min_alpha, max_alpha);
        var fadeDuration = RNG.NextDouble(min_fade_time, max_fade_time);
        var duration = RNG.NextDouble(min_scale_time, max_scale_time);
        var startSize = RNG.NextSingle(min_start_size, max_start_size);
        var extraSize = RNG.NextSingle(min_extra_size, max_extra_size);
        var x = DrawWidth * RNG.NextSingle(1);
        var y = DrawWidth * RNG.NextSingle(1);

        var circle = new Circle
        {
            Position = new Vector2(x, y),
            Anchor = Anchor.TopLeft,
            Origin = Anchor.Centre,
            Alpha = 0,
            Colour = particleColor,
            Size = new Vector2(startSize)
        };

        circle.OnLoadComplete += _ =>
        {
            circle.FadeTo(alpha, fadeDuration).ResizeTo(startSize + extraSize, duration + fadeDuration)
                  .Delay(duration).FadeOut(fadeDuration).Expire();
        };

        AddInternal(circle);

        lastSpawn = Time.Current;
    }
}
