using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Menu.UI.Visualizer;

public partial class MenuVisualizer : Container
{
    [Resolved]
    private GlobalClock clock { get; set; }

    private const int circle_count = 64;
    private const int max_extra_speed = 2;
    private const int max_extra_scale = 0;

    private bool loaded;
    private Bindable<bool> visualizerEnabled;
    private Bindable<bool> swayEnabled;

    public MenuVisualizer()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Alpha = .5f;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        visualizerEnabled = config.GetBindable<bool>(FluXisSetting.MainMenuVisualizer);
        swayEnabled = config.GetBindable<bool>(FluXisSetting.MainMenuVisualizerSway);
    }

    protected override void LoadComplete()
    {
        visualizerEnabled.BindValueChanged(e =>
        {
            // no need to fade in if we're not loaded yet
            if (!loaded)
            {
                Alpha = e.NewValue ? 1 : 0;
                return;
            }

            this.FadeTo(e.NewValue ? 1 : 0, 500, Easing.OutQuint);
        }, true);

        for (int i = 0; i < circle_count; i++)
        {
            ShapeBase shape = RNG.NextBool() ? new SquareShape() : new CircleShape();
            shape.X = DrawWidth * RNG.NextSingle();
            shape.Y = -DrawHeight * RNG.NextSingle();
            shape.Rotation = 360 * RNG.NextSingle();
            shape.RandomSpeed = 1 + RNG.NextSingle() * max_extra_speed;
            shape.Scale = new Vector2(1 + RNG.NextSingle() * max_extra_scale);
            Add(shape);
        }

        loaded = true;
        base.LoadComplete();
    }

    protected override void Update()
    {
        float amplitude = clock.Amplitudes.Where((_, i) => i is > 0 and < 4).ToList().Average();

        foreach (var child in Children)
        {
            var shape = (ShapeBase)child;
            float move = amplitude * .2f * shape.RandomSpeed;
            move *= (float)Time.Elapsed;
            shape.Y -= move;

            shape.Rotation += (float)(amplitude * .2f * shape.RandomSpeed * Time.Elapsed);

            if (swayEnabled.Value)
            {
                float x = 40 * (float)Math.Sin(Time.Current * .001f * shape.RandomSpeed) * .01f;
                x *= amplitude * shape.RandomSpeed;
                x *= (float)Time.Elapsed;
                shape.X += x;
            }

            if (!(shape.Y < -DrawHeight - shape.Height / 2f)) continue;

            shape.X = DrawWidth * RNG.NextSingle();
            shape.Y += DrawHeight + shape.Height;
            shape.RandomSpeed = 1 + RNG.NextSingle() * max_extra_speed;
            shape.Scale = new Vector2(1 + RNG.NextSingle() * max_extra_scale);
        }

        base.Update();
    }

    private abstract partial class ShapeBase : Container
    {
        public float RandomSpeed { get; set; }

        protected ShapeBase()
        {
            Size = new Vector2(32);
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.Centre;
            Blending = BlendingParameters.Additive;
            Alpha = .1f;

            Child = CreateShape().With(d =>
            {
                d.RelativeSizeAxes = Axes.Both;
                d.Masking = true;
                d.BorderThickness = 9;
                d.BorderColour = Colour4.DarkGray;
            }).WithChild(new Box
            {
                AlwaysPresent = true,
                Alpha = 0,
                RelativeSizeAxes = Axes.Both
            });
        }

        protected abstract Container CreateShape();
    }

    private partial class CircleShape : ShapeBase
    {
        protected override Container CreateShape() => new CircularContainer();
    }

    private partial class SquareShape : ShapeBase
    {
        protected override Container CreateShape() => new();
    }
}
