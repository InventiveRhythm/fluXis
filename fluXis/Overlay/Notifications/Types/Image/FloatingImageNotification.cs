using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Overlay.Notifications.Floating;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Notifications.Types.Image;

public abstract partial class FloatingImageNotification : FloatingNotification
{
    private Container animationContainer;
    private FillFlowContainer content;
    private LoadingIcon loadingIcon;
    private Container imageContainer;

    private Sample appear;
    private Sample disappear;

    protected ImageNotificationData Data { get; }

    protected FloatingImageNotification(ImageNotificationData data)
    {
        Data = data;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        appear = samples.Get("UI/Notifications/in.mp3");
        disappear = samples.Get("UI/Notifications/out.mp3");

        AutoSizeAxes = Axes.X;
        Child = animationContainer = new Container
        {
            AutoSizeAxes = Axes.Both,
            AutoSizeDuration = 400,
            AutoSizeEasing = Easing.OutQuint,
            CornerRadius = 20,
            Masking = true,
            EdgeEffect = FluXisStyles.ShadowSmall,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                loadingIcon = new LoadingIcon
                {
                    Size = new Vector2(30),
                    Margin = new MarginPadding(10),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(10),
                    Spacing = new Vector2(0, 10),
                    Alpha = 0,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = Data.Text,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        },
                        imageContainer = new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            CornerRadius = 10,
                            Masking = true,
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        appear?.Play();
        animationContainer.MoveToY(-90).MoveToY(0, 400, Easing.OutQuint);

        // load image, then play appear animation
        LoadComponentAsync(CreateImage(), image =>
        {
            imageContainer.Child = image;
            loadingIcon.FadeOut(200);
            content.FadeIn(200);

            this.Delay(5000).FadeOut(400).OnComplete(_ =>
            {
                disappear?.Play();
                Expire();
            });
        });
    }

    protected abstract Drawable CreateImage();

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        appear?.Dispose();
        disappear?.Dispose();
    }
}
