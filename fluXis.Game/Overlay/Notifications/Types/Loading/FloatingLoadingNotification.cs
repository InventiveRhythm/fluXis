using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications.Floating;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Notifications.Types.Loading;

public partial class FloatingLoadingNotification : FloatingNotification
{
    private LoadingNotificationData data { get; }

    private Container animationContainer;
    private Box background;
    private Box foreground;
    private FluXisSpriteText text;
    private SpriteIcon icon;

    private double inactivityTime;
    private bool canMinimize;
    private bool minimized;

    private readonly Color4 backgroundLoading = Color4Extensions.FromHex("#337799");
    private readonly Color4 foregroundLoading = Color4Extensions.FromHex("#66AACC");
    private readonly Color4 backgroundFailed = Color4Extensions.FromHex("#CC6666");
    private readonly Color4 backgroundComplete = Color4Extensions.FromHex("#66CC66");

    private Sample appear;
    private Sample disappear;

    public FloatingLoadingNotification(LoadingNotificationData data)
    {
        this.data = data;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        appear = samples.Get("UI/Notifications/in.mp3");
        disappear = samples.Get("UI/Notifications/out.mp3");

        AutoSizeAxes = Axes.X;
        Child = animationContainer = new CircularContainer
        {
            Size = new Vector2(50),
            Masking = true,
            EdgeEffect = FluXisStyles.ShadowSmall,
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = backgroundLoading
                },
                foreground = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0,
                    Colour = foregroundLoading
                },
                text = new FluXisSpriteText
                {
                    Text = data.TextLoading,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = -100
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(20),
                    Icon = data.Icon
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        Show();

        data.StateBindable.BindValueChanged(e => Schedule(() => onStateChange(e)));
        data.ProgressBindable.BindValueChanged(e => Schedule(() => onProgressChange(e)));
    }

    private void onProgressChange(ValueChangedEvent<float> e)
    {
        if (data.State == LoadingState.Working) foreground.ResizeWidthTo(e.NewValue, 200, Easing.OutQuint);
    }

    private void onStateChange(ValueChangedEvent<LoadingState> e)
    {
        switch (e.NewValue)
        {
            case LoadingState.Working:
                background.Colour = backgroundLoading;
                foreground.Colour = foregroundLoading;
                foreground.ResizeWidthTo(data.Progress, 200, Easing.OutQuint);
                canMinimize = true;
                text.Text = data.TextLoading;
                break;

            case LoadingState.Complete:
                background.Colour = backgroundComplete;
                foreground.ResizeWidthTo(0, 200, Easing.OutQuint);
                text.Text = data.TextSuccess;
                maximize();
                Hide(5000);
                break;

            case LoadingState.Failed:
                background.Colour = backgroundFailed;
                foreground.ResizeWidthTo(0, 200, Easing.OutQuint);
                text.Text = data.TextFailure;
                maximize();
                Hide(10000);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (canMinimize)
        {
            inactivityTime += Time.Elapsed;

            if (inactivityTime > 4000 && !minimized) minimize();
            if (inactivityTime < 4000 && minimized) maximize();
        }
    }

    public override void Show()
    {
        appear?.Play();

        this.ResizeHeightTo(50, 400, Easing.OutQuint).FadeInFromZero(400);
        animationContainer.MoveToY(-70).MoveToY(0, 400, Easing.OutQuint);
        animationContainer.Delay(900).ResizeWidthTo(text.DrawWidth + 40, 400, Easing.OutQuint);
        text.Delay(900).FadeInFromZero(400, Easing.OutQuint).MoveToY(0);
        icon.Delay(900).FadeOut(400, Easing.OutQuint);

        this.Delay(1000).Schedule(() => canMinimize = true);
    }

    public void Hide(float delay = 0)
    {
        canMinimize = false;
        this.Delay(delay).Schedule(() =>
        {
            disappear?.Play();
            this.FadeOut(400).Expire();
        });
    }

    protected override bool OnHover(HoverEvent e)
    {
        inactivityTime = 0;
        return true;
    }

    private void minimize()
    {
        minimized = true;
        this.ResizeHeightTo(20, 400, Easing.OutQuint);
        animationContainer.ResizeHeightTo(20, 400, Easing.OutQuint).ResizeWidthTo(200, 400, Easing.OutQuint);
        text.FadeOut(400, Easing.OutQuint);
    }

    private void maximize()
    {
        minimized = false;
        this.ResizeHeightTo(50, 400, Easing.OutQuint);
        animationContainer.ResizeHeightTo(50, 400, Easing.OutQuint).ResizeWidthTo(text.DrawWidth + 40, 400, Easing.OutQuint);
        text.FadeIn(400, Easing.OutQuint);
    }
}
