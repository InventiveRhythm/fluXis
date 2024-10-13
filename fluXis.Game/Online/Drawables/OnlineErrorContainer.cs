using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Online.Drawables;

public partial class OnlineErrorContainer : FillFlowContainer
{
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

    public override void Show() => this.ScaleTo(1.1f).FadeIn(FluXisScreen.FADE_DURATION).ScaleTo(1, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    public override void Hide() => this.FadeOut(FluXisScreen.FADE_DURATION).ScaleTo(.9f, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
}
