using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Notifications.Floating;

public partial class FloatingTextNotification : FloatingNotification
{
    public string Text { get; set; }
    public string SubText { get; set; }
    public IconUsage Icon { get; set; } = FontAwesome.Solid.Info;
    public Color4 BackgroundColor { get; set; } = FluXisColors.Background2;
    public float Lifetime { get; set; } = 5000;

    public string SampleAppearing { get; set; } = "UI/Notifications/in.mp3";
    public string SampleDisappearing { get; set; } = "UI/Notifications/out.mp3";

    private Container animationContainer;
    private FillFlowContainer textContainer;
    private SpriteIcon icon;

    private Sample appear;
    private Sample disappear;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        appear = samples.Get(SampleAppearing);
        disappear = samples.Get(SampleDisappearing);

        AutoSizeAxes = Axes.X;
        Child = animationContainer = new CircularContainer
        {
            Size = new Vector2(50),
            Masking = true,
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0.25f),
                Radius = 5,
                Offset = new Vector2(0, 2)
            },
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = BackgroundColor
                },
                textContainer = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Horizontal = 20 },
                    Y = -100,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = Text,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        },
                        new FluXisSpriteText
                        {
                            Text = SubText,
                            FontSize = 14,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        }
                    }
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(20),
                    Icon = Icon
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        appear?.Play();

        animationContainer.MoveToY(-70).MoveToY(0, 400, Easing.OutQuint);
        animationContainer.Delay(900).ResizeWidthTo(textContainer.DrawWidth, 400, Easing.OutQuint);
        textContainer.Delay(900).FadeInFromZero(400, Easing.OutQuint).MoveToY(0);
        icon.Delay(900).FadeOut(400, Easing.OutQuint);

        this.ResizeHeightTo(50, 400, Easing.OutQuint).FadeInFromZero(400).Then(Lifetime).FadeOut(400).OnComplete(_ =>
        {
            disappear?.Play();
            Expire();
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        appear?.Dispose();
        disappear?.Dispose();
    }
}
