using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.UI;

public partial class EditorOsd : CompositeDrawable
{
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;
        CornerRadius = 16;
        Masking = true;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background1,
                Alpha = 0.75f
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 32, Vertical = 24 },
                Child = text = new FluXisSpriteText
                {
                    WebFontSize = 20,
                }
            }
        };
    }

    public void DisplayText(LocalisableString str)
    {
        text.Text = str;

        this.FadeIn(50).ScaleTo(1.1f)
            .ScaleTo(1f, 300, Easing.OutQuint)
            .Delay(1000).FadeOut(150)
            .ScaleTo(0.9f, 300, Easing.OutQuint);
    }
}
