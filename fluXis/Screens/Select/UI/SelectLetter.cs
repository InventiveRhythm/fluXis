using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Select.UI;

public partial class SelectLetter : CompositeDrawable
{
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Size = new Vector2(96);
        Alpha = 0;
        CornerRadius = 16;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Alpha = .8f,
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                WebFontSize = 48
            }
        };
    }

    public void SetLetter(char letter, bool found = true)
    {
        text.Text = letter.ToString();

        this.ScaleTo(1.1f).FadeIn(50)
            .ScaleTo(1, 200, Easing.OutQuint).Delay(600)
            .FadeOut(200).ScaleTo(.9f, 400, Easing.OutQuint);

        if (!found)
            text.FadeColour(Theme.Red).Delay(200).FadeColour(Theme.Text, 600);
    }
}
