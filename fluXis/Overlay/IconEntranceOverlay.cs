using System.Collections.Generic;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input.Focus;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Overlay;

public abstract partial class IconEntranceOverlay : OverlayContainer
{
    protected override bool PlaySamples => false;

    protected virtual float OverlayPadding => 64;
    protected virtual float IconRotation => 60;
    protected virtual float OpenedRoundness => 20;
    protected virtual ColourInfo BackgroundColor => Theme.Background2;
    protected virtual IconUsage Icon => Phosphor.Bold.QuestionMark;
    protected virtual float MaxWidth => float.MaxValue;

    [CanBeNull]
    protected Sample OpenSample { get; set; }

    [CanBeNull]
    protected Sample CloseSample { get; set; }

    protected bool InitialAnimation { get; private set; } = true;

    private Container container;
    private ClickableContainer scaling;
    private Container iconContainer;
    protected new FocusContainer Content { get; private set; }

    private float animation = 0f;

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
            container = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Child = scaling = new ClickableContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    CornerRadius = 20,
                    Masking = true,
                    EdgeEffect = Styling.ShadowLarge,
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
                        Content = new FocusContainer
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

        const int scale_duration = 400;

        this.TransformTo(nameof(animation), 0f)
            .Delay(scale_duration)
            .TransformTo(nameof(animation), 1f, 600, Easing.OutQuint);

        scaling.ScaleTo(.8f).TransformTo(nameof(CornerRadius), 20f)
               .ScaleTo(1, scale_duration, Easing.OutQuint)
               .Delay(scale_duration)
               .TransformTo(nameof(CornerRadius), OpenedRoundness);

        iconContainer.FadeIn().RotateTo(-IconRotation)
                     .RotateTo(0, scale_duration, Easing.OutQuint)
                     .Delay(scale_duration)
                     .FadeOut(200).RotateTo(IconRotation, scale_duration, Easing.OutQuint);

        Content.FadeOut().Then(scale_duration + 400).FadeIn(200);
        this.FadeInFromZero(200);
    }

    protected override void Update()
    {
        base.Update();

        var padded = container.DrawSize - new Vector2(OverlayPadding * 2);
        if (padded.X > MaxWidth) padded = new Vector2(MaxWidth, padded.Y);

        scaling.Size = new Vector2(
            (float)Interpolation.Lerp(200, padded.X, animation),
            (float)Interpolation.Lerp(200, padded.Y, animation)
        );
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

    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;
    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnMouseDown(MouseDownEvent e) => true;
}
