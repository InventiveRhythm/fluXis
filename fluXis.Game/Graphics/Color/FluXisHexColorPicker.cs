using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Graphics.Color;

public partial class FluXisHexColorPicker : HexColourPicker
{
    protected override TextBox CreateHexCodeTextBox() => new FluXisTextBox { Height = 40 };
    protected override ColourPreview CreateColourPreview() => new FluXisColorPreview();

    public FluXisHexColorPicker()
    {
        Padding = new MarginPadding(20);
        Spacing = 20;
        Background.Colour = FluXisColors.Surface2;
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
