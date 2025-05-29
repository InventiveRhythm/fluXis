using System.Collections.Generic;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay;

public abstract partial class IconEntranceOverlay : OverlayContainer
{
    protected override bool PlaySamples => false;

    protected virtual float OverlayPadding => 64;
    protected virtual float IconRotation => 60;
    protected virtual ColourInfo BackgroundColor => FluXisColors.Background2;
    protected virtual IconUsage Icon => FontAwesome6.Solid.Question;

    [CanBeNull]
    protected Sample OpenSample { get; set; }

    [CanBeNull]
    protected Sample CloseSample { get; set; }

    protected bool InitialAnimation { get; private set; } = true;

    private ClickableContainer scaling;
    private Container iconContainer;
    private Container content;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Action = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = .5f
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(OverlayPadding),
                Child = scaling = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    CornerRadius = 20,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowLarge,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = BackgroundColor
                        },
                        iconContainer = new Container
                        {
                            Size = new Vector2(200),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Child = new FluXisSpriteIcon
                            {
                                RelativeSizeAxes = Axes.Both,
                                Size = new Vector2(.5f),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Icon = Icon
                            }
                        },
                        content = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            ChildrenEnumerable = CreateContent()
                        }
                    }
                }
            }
        };
    }

    protected abstract IEnumerable<Drawable> CreateContent();

    protected override void PopIn()
    {
        if (!InitialAnimation)
            OpenSample?.Play();

        const int size = 200;
        const int scale_duration = 400;

        var widthFactor = size / DrawSize.X;
        var heightFactor = size / DrawSize.Y;

        scaling.ScaleTo(.8f)
               .ResizeTo(new Vector2(widthFactor, heightFactor))
               .ScaleTo(1, scale_duration, Easing.OutQuint)
               .Delay(scale_duration)
               .ResizeTo(1, 600, Easing.OutQuint);

        iconContainer.FadeIn().RotateTo(-IconRotation)
                     .RotateTo(0, scale_duration, Easing.OutQuint)
                     .Delay(scale_duration)
                     .FadeOut(200).RotateTo(IconRotation, scale_duration, Easing.OutQuint);

        content.FadeOut().Then(scale_duration + 400).FadeIn(200);
        this.FadeInFromZero(200);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        InitialAnimation = false;
    }

    protected override void PopOut()
    {
        if (!InitialAnimation)
            CloseSample?.Play();

        this.FadeOut(200);
        scaling.ScaleTo(.95f, 400, Easing.OutQuint);
    }
}
