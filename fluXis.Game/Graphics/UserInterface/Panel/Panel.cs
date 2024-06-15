using fluXis.Game.Audio;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class Panel : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    public bool IsDangerous { get; init; }

    public new Container Content { get; }
    public bool ShowOnCreate { get; set; } = true;

    private Box flashBox { get; }

    public Panel()
    {
        Masking = true;
        CornerRadius = 20;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        EdgeEffect = FluXisStyles.ShadowMedium;

        Children = new Drawable[]
        {
            new PanelBackground(),
            flashBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Colour = FluXisColors.Red
            },
            Content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20)
            }
        };
    }

    protected override void LoadComplete()
    {
        if (ShowOnCreate) Show();
    }

    public void Flash()
    {
        flashBox.FadeOutFromOne(1000, Easing.OutQuint);
    }

    public override void Hide()
    {
        samples.PanelClose(IsDangerous);

        this.ScaleTo(.9f, 400, Easing.OutQuint)
            .FadeOut(400, Easing.OutQuint);
    }

    public override void Show()
    {
        samples.PanelOpen(IsDangerous);

        this.RotateTo(0).ScaleTo(.75f)
            .FadeInFromZero(400, Easing.OutQuint)
            .ScaleTo(1f, 800, Easing.OutElasticHalf);
    }
}
