using fluXis.Graphics;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Online.Drawables;

public partial class OnlineErrorContainer : FillFlowContainer
{
    public bool ShowInstantly { get; init; }

    public LocalisableString Text
    {
        get => text.Text;
        set => text.Text = value;
    }

    private FluXisSpriteText text { get; }

    public OnlineErrorContainer()
        : this("Something went wrong...")
    {
    }

    public OnlineErrorContainer(LocalisableString title)
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(-2);
        AlwaysPresent = true;
        Alpha = 0;

        Children = new Drawable[]
        {
            new FluXisSpriteText
            {
                WebFontSize = 16,
                Text = title,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            text = new FluXisSpriteText
            {
                WebFontSize = 12,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Alpha = .8f
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (ShowInstantly)
            Show();
    }

    public override void Show() => this.ScaleTo(1.1f).FadeIn(Styling.TRANSITION_FADE).ScaleTo(1, Styling.TRANSITION_MOVE, Easing.OutQuint);
    public override void Hide() => this.FadeOut(Styling.TRANSITION_FADE).ScaleTo(.9f, Styling.TRANSITION_MOVE, Easing.OutQuint);
}
