using osu.Framework.Graphics.UserInterface;

namespace fluXis.Graphics.UserInterface.Color;

public partial class FluXisColorPicker : ColourPicker
{
    protected override HSVColourPicker CreateHSVColourPicker() => new FluXisHsvColourPicker();

    protected override HexColourPicker CreateHexColourPicker() => new FluXisHexColorPicker();
}
