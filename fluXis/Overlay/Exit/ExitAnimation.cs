using System;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Overlay.Exit;

public partial class ExitAnimation : FullInputBlockingContainer
{
    private const int bars = 8;
    private const int bars_duration = 800;
    private const int bars_delay = 200;

    private const string goodbye = "Goodbye!";
    private const string scramble_chars = "ABCDEFGHIJKMNOPQRSTUVWXYZ@#$%&*()+=[]{}~";

    private Sample sample;

    private Container barsContainer;
    private SpriteText text;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        sample = samples.Get("Intro/exit");

        InternalChildren = new Drawable[]
        {
            barsContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Scale = new Vector2(1.2f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 10,
                ChildrenEnumerable = Enumerable.Range(0, bars).Select(i => new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    MaskingSmoothness = 0,
                    Width = 1f / bars,
                    X = i / (float)bars - .001f * i, // stupid, but works
                    Y = 1.6f,
                    Height = 1.6f,
                    Colour = Colour4.Black
                })
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = getScrambled(),
                FontSize = 50,
                Alpha = 0
            }
        };
    }

    public void Show(Action onBarCompletion, Action onCompletion)
    {
        base.Show();

        sample.Play();

        foreach (var bar in barsContainer.Children)
        {
            var rngDelay = RNG.Next(0, bars_delay);
            bar.Delay(rngDelay).MoveToY(0, bars_duration, Easing.OutCirc);
        }

        barsContainer.Delay(bars_duration + bars_delay).Schedule(onBarCompletion);

        text.ScaleTo(1.4f).Delay(bars_duration)
            .Schedule(() => Scheduler.AddDelayed(unscrambleOneRandomChar, 50, true))
            .FadeIn(400).ScaleTo(1, 800, Easing.OutQuint)
            .Then(600).FadeOut(600).OnComplete(_ => onCompletion());
    }

    private void unscrambleOneRandomChar()
    {
        if (text.Text == goodbye) return;

        var chars = text.Text.ToString().ToCharArray();
        var idx = RNG.Next(0, chars.Length);

        while (chars[idx] == goodbye[idx])
            idx = RNG.Next(0, chars.Length);

        chars[idx] = goodbye[idx];
        text.Text = new string(chars);
    }

    private static string getScrambled()
    {
        var chars = goodbye.ToCharArray();
        var scramble = new char[chars.Length];

        for (var i = 0; i < chars.Length; i++)
        {
            var idx = RNG.Next(0, scramble_chars.Length);
            scramble[i] = scramble_chars[idx];
        }

        return new string(scramble);
    }
}
