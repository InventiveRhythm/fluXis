using System;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI.Visualizer;

public partial class MenuVisualizer : Container
{
    // between 60 and 240
    private const int start_index = 1; // 78
    private const int end_index = 3; // 234

    private const int circle_count = 64;

    private float amplitude;
    private bool loaded;
    private Bindable<bool> visualizerEnabled;

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

        var rng = new Random();

        for (int i = 0; i < circle_count; i++)
        {
            float x = DrawWidth * rng.NextSingle();

            Add(new DotCircle
            {
                X = x,
                Y = -DrawHeight * rng.NextSingle(),
                RandomSpeed = 1 + rng.NextSingle() * 2
            });
        }

        loaded = true;
        base.LoadComplete();
    }

    protected override void Update()
    {
        updateAmplitude();

        foreach (var child in Children)
        {
            var circle = (DotCircle)child;
            float move = amplitude * .2f * circle.RandomSpeed;
            move *= (float)Time.Elapsed;
            circle.Y -= move;

            /*float x = 40 * (float)Math.Sin(Time.Current * .1f) * .001f;
            x *= amplitude * circle.RandomSpeed;
            circle.X += x;*/

            if (circle.Y < -DrawHeight - circle.Height / 2f)
            {
                circle.X = DrawWidth * RNG.NextSingle();
                circle.Y += DrawHeight + circle.Height;
                circle.RandomSpeed = 1 + RNG.NextSingle() * 2;
            }
        }

        base.Update();
    }

    private void updateAmplitude()
    {
        float[] span = Conductor.Track?.CurrentAmplitudes.FrequencyAmplitudes.Span.ToArray() ?? new float[256];

        float newAmplitude = 0;

        for (var i = 0; i < span.Length; i++)
        {
            if (i is >= start_index and <= end_index)
                newAmplitude += span[i];
        }

        newAmplitude /= end_index - start_index;

        // smooth out the amplitude (less smoothing for amplitude increase, more for decrease)
        float delta = newAmplitude - amplitude;
        float interpolation = delta < 0
            ? (float)Math.Pow(.01, Time.Elapsed / 1000f)
            : (float)Math.Pow(.1, Time.Elapsed / 1000f);

        amplitude += delta * interpolation;
    }

    private partial class DotCircle : CircularContainer
    {
        public float RandomSpeed { get; set; }

        public DotCircle()
        {
            Size = new Vector2(32);
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.Centre;
            Blending = BlendingParameters.Additive;
            Masking = true;
            BorderThickness = 9;
            BorderColour = Colour4.DarkGray;
            Alpha = .1f;

            Child = new Box
            {
                AlwaysPresent = true,
                Alpha = 0,
                RelativeSizeAxes = Axes.Both
            };
        }
    }
}
