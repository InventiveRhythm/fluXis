using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

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
        Scale = new Vector2(.95f);
        Rotation = 2;
        Alpha = 0;
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
        this.ScaleTo(.95f, 400, Easing.OutQuint).RotateTo(2, 400, Easing.OutQuint).FadeOut(200);
    }

    public override void Show()
    {
        this.RotateTo(0).ScaleTo(1f, 800, Easing.OutElastic).FadeIn(200);
    }
}
