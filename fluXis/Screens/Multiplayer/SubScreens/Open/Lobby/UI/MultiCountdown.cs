using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.Multiplayer;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI;

public partial class MultiCountdown : CompositeDrawable
{
    [Resolved]
    private MultiplayerClient client { get; set; }

    private Sample tick;
    private Sample warn;
    private Sample final;

    private FluXisSpriteText text;
    private OutlinedCircle circle;
    private OutlinedSquare screenPulse;
    private long endTime;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        tick = samples.Get("Multiplayer/countdown-tick-1");
        warn = samples.Get("Multiplayer/countdown-warn");
        final = samples.Get("Multiplayer/countdown-warn-final");

        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            circle = new OutlinedCircle
            {
                BorderThickness = 0,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                BorderColour = FluXisColors.Red,
            },
            text = new FluXisSpriteText
            {
                WebFontSize = 72,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shadow = true,
                Alpha = 0
            },
            screenPulse = new OutlinedSquare
            {
                RelativeSizeAxes = Axes.Both,
                BorderThickness = 0,
                BorderColour = FluXisColors.Red
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (endTime <= 0)
            return;

        var timeLeft = endTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var seconds = (int)Math.Ceiling(timeLeft / 1000d);
        var leftInSecond = 1000 - (seconds * 1000f - timeLeft);

        var display = $"{seconds}";

        if (display == text.Text || seconds <= 0)
            return;

        var scale = 0.8f;
        text.Colour = FluXisColors.Red;

        switch (seconds)
        {
            case 1:
                final?.Play();
                scale = 1.2f;

                circle.BorderTo(24).ResizeTo(0)
                      .ResizeTo(600, 1000, Easing.OutQuint)
                      .BorderTo(0, 1000, Easing.Out);

                screenPulse.BorderTo(24)
                           .BorderTo(0, 1000, Easing.Out);

                break;

            case <= 3:
                warn?.Play();
                scale = 1f;

                circle.BorderTo(12).ResizeTo(0)
                      .ResizeTo(300, 1000, Easing.OutQuint)
                      .BorderTo(0, 1000, Easing.Out);

                screenPulse.BorderTo(12)
                           .BorderTo(0, 1000, Easing.Out);

                break;

            default:
                tick?.Play();
                text.Colour = FluXisColors.Text;
                break;
        }

        var delay = Math.Max(leftInSecond - 200, 0);
        text.FadeInFromZero(100).ScaleTo(scale + .1f)
            .ScaleTo(scale, 300, Easing.OutQuint)
            .Delay(delay)
            .FadeOut(100)
            .ScaleTo(scale - .1f, 300, Easing.OutQuint);

        text.Text = $"{seconds}";
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        client.CountdownUpdated += updateCountdown;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        client.CountdownUpdated -= updateCountdown;
    }

    private void updateCountdown(long? time)
    {
        if (time is null)
        {
            endTime = 0;
            text.Text = "";
            text.Hide();
            return;
        }

        endTime = time.Value;
        text.Show();
    }
}
