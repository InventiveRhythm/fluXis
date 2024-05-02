using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens;

public partial class MultiModeButton : VisibilityContainer
{
    [Resolved]
    private UISamples samples { get; set; }

    protected override bool StartHidden => true;

    public string Title { get; init; }
    public string Description { get; init; }
    public string Background { get; init; }
    public bool RightSide { get; init; }
    public bool Locked { get; init; }
    public Action Action { get; init; }
    public Action HoverAction { get; init; }
    public Action HoverLostAction { get; init; }

    private const float x_offset_exit = 0.14f;
    private const float x_offset = 0.12f;
    private const float x_offset_hover = 0.11f;
    private const float y_offset = 50;

    private const float content_width = 0.6f;
    private const float inner_content_offset = x_offset / content_width;
    private const float inner_content_width = 1 - inner_content_offset;

    private Container dim;
    private Container border;

    private Sample accessDeniedSample;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, ISampleStore samples)
    {
        accessDeniedSample = samples.Get("Multiplayer/access-denied");

        RelativeSizeAxes = Axes.X;
        RelativePositionAxes = Axes.X;
        Width = content_width;
        Height = 350;
        X = RightSide ? x_offset : -x_offset;
        Y = RightSide ? -y_offset : y_offset;
        Anchor = RightSide ? Anchor.CentreRight : Anchor.CentreLeft;
        Origin = RightSide ? Anchor.CentreRight : Anchor.CentreLeft;
        Masking = true;
        CornerRadius = 40;
        Shear = new Vector2(-0.2f, 0);
        EdgeEffect = FluXisStyles.ShadowMedium;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Texture = textures.Get($"Multiplayer/button-{Background}")
            },
            dim = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .3f,
                        Anchor = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                        Origin = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                        Colour = Colour4.Black.Opacity(.7f)
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .7f,
                        Anchor = RightSide ? Anchor.CentreLeft : Anchor.CentreRight,
                        Origin = RightSide ? Anchor.CentreLeft : Anchor.CentreRight,
                        Colour = RightSide
                            ? ColourInfo.GradientHorizontal(Colour4.Black.Opacity(.4f), Colour4.Black.Opacity(.7f))
                            : ColourInfo.GradientHorizontal(Colour4.Black.Opacity(.7f), Colour4.Black.Opacity(.4f))
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = RightSide ? Axes.None : Axes.X,
                Width = inner_content_width,
                X = RightSide ? 80 : inner_content_offset,
                Shear = new Vector2(0.2f, 0),
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                    Origin = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding
                    {
                        Left = RightSide ? 0 : 100,
                        Right = RightSide ? 100 : 0
                    },
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = Title,
                            FontSize = 100,
                            Shadow = true,
                            Anchor = RightSide ? Anchor.TopRight : Anchor.TopLeft,
                            Origin = RightSide ? Anchor.TopRight : Anchor.TopLeft
                        },
                        new FluXisTextFlow
                        {
                            Text = Description,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Alpha = .8f,
                            FontSize = 40,
                            Shadow = true,
                            TextAnchor = RightSide ? Anchor.TopRight : Anchor.TopLeft,
                            Anchor = RightSide ? Anchor.TopRight : Anchor.TopLeft,
                            Origin = RightSide ? Anchor.TopRight : Anchor.TopLeft
                        }
                    }
                }
            },
            border = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 40,
                BorderThickness = 8,
                Alpha = 0,
                BorderColour = RightSide
                    ? ColourInfo.GradientHorizontal(FluXisColors.Background4, FluXisColors.Background4.Opacity(0))
                    : ColourInfo.GradientHorizontal(FluXisColors.Background4.Opacity(0), FluXisColors.Background4),
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        HoverAction?.Invoke();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        HoverLostAction?.Invoke();
    }

    protected override bool OnClick(ClickEvent e)
    {
        Trigger();
        return true;
    }

    public void Trigger()
    {
        if (Locked)
            accessDeniedSample?.Play();
        else
            samples.Click();

        HoverAction?.Invoke(); // to avoid the other button getting stuck in the hover state
        Action?.Invoke();
    }

    public void Select()
    {
        if (State.Value == Visibility.Hidden) return;

        samples.Hover();

        var x = RightSide ? x_offset_hover : -x_offset_hover;
        this.MoveToX(x, 100, Easing.OutQuint);

        border.FadeIn(50);
        dim.FadeTo(.8f, 50);
    }

    public void Deselect()
    {
        var x = RightSide ? x_offset : -x_offset;
        this.MoveToX(x, 400, Easing.OutQuint);

        border.FadeOut(200);
        dim.FadeIn(200);
    }

    protected override void PopIn() => this.MoveToX(RightSide ? x_offset : -x_offset, 400, Easing.OutQuint);
    protected override void PopOut() => this.MoveToX(RightSide ? x_offset_exit : -x_offset_exit, 400, Easing.OutQuint);
}
