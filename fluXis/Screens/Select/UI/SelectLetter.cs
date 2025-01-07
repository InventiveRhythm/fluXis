using fluXis.Graphics.Sprites;
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
        Size = new Vector2(100);
        Alpha = 0;
        CornerRadius = 20;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Alpha = 0.5f,
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FontSize = 64,
                Text = "A"
            }
        };
    }

    public void SetLetter(char letter, bool found = true)
    {
        text.Text = letter.ToString();
        this.FadeIn(200).Delay(1000).FadeOut(300);

        if (!found)
            text.FadeColour(Colour4.FromHex("#FF5555")).FadeColour(FluXisColors.Text, 1000);
    }
}
