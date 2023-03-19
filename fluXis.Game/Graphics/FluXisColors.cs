using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace fluXis.Game.Graphics;

public class FluXisColors
{
    public static Colour4 Accent = Colour4.FromHex("#3650eb");
    public static Colour4 Accent2 = Colour4.FromHex("#4846d5");
    public static Colour4 Accent3 = Colour4.FromHex("#533ec3");
    public static Colour4 Accent4 = Colour4.FromHex("#5f30a7");

    public static ColourInfo AccentGradient = ColourInfo.GradientHorizontal(Accent, Accent4);

    public static Colour4 Background = Colour4.FromHex("#1a1a20");
    public static Colour4 Background2 = Colour4.FromHex("#222228");

    public static Colour4 Hover = Colour4.FromHex("#7e7e7f");
    public static Colour4 Click = Colour4.FromHex("#ffffff");

    public static Colour4 Surface = Colour4.FromHex("#2a2a30");
    public static Colour4 Surface2 = Colour4.FromHex("#323238");
    public static Colour4 SurfaceDisabled = Colour4.FromHex("#2e2e34");

    public static Colour4 Text = Colour4.FromHex("#ffffff");
    public static Colour4 Text2 = Colour4.FromHex("#c8c8c8");
    public static Colour4 TextDisabled = Colour4.FromHex("#646464");

    public static Colour4 GetLaneColor(int lane, int keyCount)
    {
        return keyCount switch
        {
            4 => lane switch
            {
                1 or 4 => Accent,
                _ => Accent3
            },
            5 => lane switch
            {
                1 or 5 => Accent,
                2 or 4 => Accent3,
                _ => Accent4
            },
            6 => lane switch
            {
                1 or 3 or 4 or 6 => Accent3,
                _ => Accent
            },
            7 => lane switch
            {
                1 or 3 or 5 or 7 => Accent3,
                2 or 6 => Accent,
                _ => Accent4
            },
            _ => Colour4.White
        };
    }
}
