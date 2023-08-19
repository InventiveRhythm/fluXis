using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Graphics.UserInterface.Color;

public partial class FluXisColorPicker : ColourPicker
{
    public FluXisColorPicker()
    {
        CornerRadius = 10;
        Masking = true;
    }

    protected override HSVColourPicker CreateHSVColourPicker() => new FluXisHsvColourPicker();

    protected override HexColourPicker CreateHexColourPicker() => new FluXisHexColorPicker();
}
