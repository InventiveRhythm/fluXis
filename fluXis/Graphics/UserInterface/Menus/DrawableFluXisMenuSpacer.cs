using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Graphics.UserInterface.Menus;

public partial class DrawableFluXisMenuSpacer : Menu.DrawableMenuItem
{
    public DrawableFluXisMenuSpacer(FluXisMenuSpacer spacer)
        : base(spacer)
    {
        BackgroundColour = Colour4.Transparent;
        BackgroundColourHover = Colour4.Transparent;

        Foreground.AutoSizeAxes = Axes.Y;
        Foreground.RelativeSizeAxes = Axes.X;
    }

    protected override Drawable CreateContent()
    {
        return new Container
        {
            RelativeSizeAxes = Axes.X,
            Height = 2,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background6
            }
        };
    }
}
