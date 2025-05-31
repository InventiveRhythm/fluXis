using fluXis.Audio;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel;

public partial class Panel : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    public bool IsDangerous { get; init; }

    public new Container Content { get; }

    private Container loadingOverlay { get; }

    protected bool Loading { get; private set; }

    private Visibility state = Visibility.Visible;
    private bool initial = true;

    public Panel()
    {
        Masking = true;
        CornerRadius = 20;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        EdgeEffect = FluXisStyles.ShadowMedium;
        BorderColour = FluXisColors.Red;

        Children = new[]
        {
            CreateBackground(),
            Content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20)
            },
            loadingOverlay = new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2,
                        Alpha = .5f
                    },
                    new LoadingIcon
                    {
                        Size = new Vector2(48),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected virtual Drawable CreateBackground() => new PanelBackground();

    protected override void LoadComplete()
    {
        if (state == Visibility.Hidden)
            return;

        Show();
        initial = true;
    }

    public void Flash()
    {
        this.BorderTo(12).BorderTo(0, 800, Easing.OutQuint);
        this.Shake(100, 20, 2);
        samples.PanelOpen(true);
    }

    public override void Hide()
    {
        if (state == Visibility.Hidden)
            return;

        state = Visibility.Hidden;
        samples.PanelClose(IsDangerous);

        this.ScaleTo(.9f, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
            .RotateTo(-5, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
            .FadeOut(FluXisScreen.FADE_DURATION, Easing.OutQuint);
    }

    public override void Show()
    {
        if (state == Visibility.Visible && !initial)
            return;

        state = Visibility.Visible;
        samples.PanelOpen(IsDangerous);

        this.RotateTo(15).ScaleTo(1.25f)
            .RotateTo(0f, 800, Easing.OutElasticHalf)
            .FadeInFromZero(FluXisScreen.FADE_DURATION, Easing.OutQuint)
            .ScaleTo(1f, 800, Easing.OutElasticHalf);
    }

    public void StartLoading()
    {
        Loading = true;
        loadingOverlay.FadeIn(300);
    }

    public void StopLoading(bool hide = true)
    {
        Loading = false;
        loadingOverlay.FadeOut(300);

        if (hide)
            Hide();
    }
}
