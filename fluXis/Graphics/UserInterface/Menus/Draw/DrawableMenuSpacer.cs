using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus.Items;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Graphics.UserInterface.Menus.Draw;

public partial class DrawableMenuSpacer : Menu.DrawableMenuItem
{
    public DrawableMenuSpacer(MenuSpacerItem spacer)
        : base(spacer)
    {
        BackgroundColour = Colour4.Transparent;
        BackgroundColourHover = Colour4.Transparent;

        Foreground.AutoSizeAxes = Axes.Y;
        Foreground.RelativeSizeAxes = Axes.X;
    }

    protected override Drawable CreateContent() => new Container
    {
        RelativeSizeAxes = Axes.X,
        Height = 2,
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Theme.Background6
        }
    };
}
