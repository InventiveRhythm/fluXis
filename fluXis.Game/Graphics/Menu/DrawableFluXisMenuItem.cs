using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Graphics.Menu;

public partial class DrawableFluXisMenuItem : osu.Framework.Graphics.UserInterface.Menu.DrawableMenuItem
{
    protected virtual float TextSize => 16;

    private FluXisSpriteText text;

    public DrawableFluXisMenuItem(MenuItem item)
        : base(item)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        BackgroundColour = Colour4.Transparent;
        BackgroundColourHover = Colour4.White.Opacity(.2f);
        CornerRadius = 5;
        Masking = true;

        updateColor();
    }

    private void updateColor()
    {
        switch ((Item as FluXisMenuItem)?.Type)
        {
            default:
                text.Colour = FluXisColors.Text;
                break;

            case MenuItemType.Highlighted:
                text.Colour = FluXisColors.Accent;
                break;

            case MenuItemType.Dangerous:
                text.Colour = Colour4.FromHex("#ff5555");
                break;
        }
    }

    protected override Drawable CreateContent()
    {
        return text = new FluXisSpriteText
        {
            FontSize = TextSize,
            Margin = new MarginPadding { Vertical = 2, Horizontal = 5 },
            Shadow = true
        };
    }
}
