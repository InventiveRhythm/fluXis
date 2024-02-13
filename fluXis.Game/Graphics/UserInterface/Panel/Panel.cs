using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class Panel : Container
{
    public new Container Content { get; }
    public bool ShowOnCreate { get; set; } = true;

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

    public override void Hide()
    {
        this.ScaleTo(.9f, 400, Easing.OutQuint)
            .FadeOut(400, Easing.OutQuint);
    }

    public override void Show()
    {
        this.RotateTo(0).ScaleTo(.75f)
            .FadeInFromZero(400, Easing.OutQuint)
            .ScaleTo(1f, 800, Easing.OutElasticHalf);
    }
}
