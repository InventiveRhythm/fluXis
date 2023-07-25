using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Notification;

public partial class LoadingNotification : SimpleNotification
{
    public LoadingState State
    {
        get => state;
        set
        {
            state = value;
            Schedule(() =>
            {
                switch (state)
                {
                    case LoadingState.Loading:
                        Text = TextLoading;
                        background.FadeColour(Colour4.LightBlue, 500);
                        background.FadeTo(.25f, 300).Then().FadeTo(.15f, 300).Loop();
                        if (ShowProgress) background.ResizeWidthTo(0, 0, Easing.OutQuint);

                        loadingIcon.FadeIn();
                        successIcon.FadeOut();
                        failureIcon.FadeOut();
                        break;

                    case LoadingState.Loaded:
                        Text = TextSuccess;
                        background.FadeColour(Colour4.LightGreen, 500);
                        background.FadeTo(.25f, 200);
                        background.ResizeWidthTo(1, 300, Easing.OutQuint);
                        CloseButton.FadeTo(.4f, 200);

                        loadingIcon.FadeOut();
                        successIcon.FadeIn();
                        failureIcon.FadeOut();
                        break;

                    case LoadingState.Failed:
                        Text = TextFailure;
                        background.FadeColour(Colour4.Red, 500);
                        background.FadeTo(.25f, 200);
                        background.ResizeWidthTo(1, 300, Easing.OutQuint);
                        CloseButton.FadeTo(.4f, 200);

                        loadingIcon.FadeOut();
                        successIcon.FadeOut();
                        failureIcon.FadeIn();
                        break;
                }
            });
        }
    }

    private LoadingState state = LoadingState.Loading;
    private readonly Box background;

    public override bool ShowCloseButton => false;

    public string TextLoading { get; set; } = "Loading...";
    public string TextSuccess { get; set; } = "Loaded!";
    public string TextFailure { get; set; } = "Failed!";

    public float Progress
    {
        set
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");

            Schedule(() =>
            {
                if (State == LoadingState.Loading)
                    background.ResizeWidthTo(value, 300, Easing.OutQuint);
            });
        }
    }

    public bool ShowProgress { get; set; }

    private SpriteIcon loadingIcon;
    private SpriteIcon successIcon;
    private SpriteIcon failureIcon;

    public LoadingNotification()
    {
        Background.Add(background = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.LightBlue,
            Alpha = .25f
        });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Text = state switch
        {
            LoadingState.Loaded => TextSuccess,
            LoadingState.Failed => TextFailure,
            _ => TextLoading
        };

        if (State == LoadingState.Loading)
        {
            background.FadeTo(.25f, 300).Then().FadeTo(.15f, 300).Loop();

            if (ShowProgress)
                background.ResizeWidthTo(0, 0, Easing.OutQuint);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        IconContainer.Clear();
        IconContainer.AddRange(new[]
        {
            loadingIcon = new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome.Solid.CircleNotch,
                Size = new Vector2(20),
                Alpha = 0
            },
            successIcon = new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome.Solid.CheckCircle,
                Size = new Vector2(20),
                Alpha = 0
            },
            failureIcon = new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome.Solid.ExclamationTriangle,
                Size = new Vector2(20),
                Alpha = 0
            }
        });

        loadingIcon.Spin(1000, RotationDirection.Clockwise);

        switch (State)
        {
            case LoadingState.Loaded:
                successIcon.FadeIn();
                break;

            case LoadingState.Failed:
                failureIcon.FadeIn();
                break;

            default:
                loadingIcon.FadeIn();
                break;
        }
    }

    protected override void Update()
    {
        if (State != LoadingState.Loading)
            base.Update();
    }
}

public enum LoadingState
{
    Loading,
    Loaded,
    Failed
}
