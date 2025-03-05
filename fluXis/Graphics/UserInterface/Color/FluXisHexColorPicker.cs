using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Graphics.UserInterface.Color;

public partial class FluXisHexColorPicker : HexColourPicker
{
    protected override TextBox CreateHexCodeTextBox() => new FluXisTextBox
    {
        FontSize = FluXisSpriteText.GetWebFontSize(14),
        FixedWidth = true,
        Height = 32
    };

    protected override ColourPreview CreateColourPreview() => new FluXisColorPreview();

    public FluXisHexColorPicker()
    {
        Padding = new MarginPadding(16) { Top = 8 };
        Spacing = 16;
        Background.Colour = Colour4.Transparent;
    }

    private partial class FluXisColorPreview : ColourPreview
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Box box;

            InternalChild = new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Child = box = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            };

            Current.BindValueChanged(e =>
            {
                box.Colour = e.NewValue;
            }, true);
        }
    }
}
