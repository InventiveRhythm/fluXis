using fluXis.Mods;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osuTK;

namespace fluXis.Graphics.UserInterface.Color;

public static class Theme
{
    public static Colour4 Primary => Colour4.FromHex("#6F6FE2");
    public static Colour4 Secondary => Colour4.FromHex("#AF59CF");

    public static ColourInfo AccentGradient => ColourInfo.GradientHorizontal(Primary, Secondary);

    public static Colour4 Background1 => getThemeColor(.2f, .12f);
    public static Colour4 Background2 => getThemeColor(.2f, .15f);
    public static Colour4 Background3 => getThemeColor(.2f, .18f);
    public static Colour4 Background4 => getThemeColor(.2f, .21f);
    public static Colour4 Background5 => getThemeColor(.2f, .24f);
    public static Colour4 Background6 => getThemeColor(.2f, .27f);
    public static Colour4 Foreground => getThemeColor(.2f, .6f);
    public static Colour4 Highlight => getThemeColor(.6f, .7f);

    private static Colour4 getThemeColor(float saturation, float lightness) => Colour4.FromHSL(240 / 360f, saturation, lightness);

    public static Colour4 Text => Colour4.FromHex("#E1E2F8");
    public static Colour4 Text2 => Text.Opacity(.8f);
    public static Colour4 TextDark => Background3;

    public static Colour4 Red => Colour4.FromHSL(0f, 1f, 2 / 3f);
    public static Colour4 Orange => Colour4.FromHSL(20 / 360f, 1f, 2 / 3f);
    public static Colour4 Yellow => Colour4.FromHSL(40 / 360f, 1f, 2 / 3f);
    public static Colour4 Lime => Colour4.FromHSL(80 / 360f, 1f, 2 / 3f);
    public static Colour4 Green => Colour4.FromHSL(120 / 360f, 1f, 2 / 3f);
    public static Colour4 Aqua => Colour4.FromHSL(160 / 360f, 1f, 2 / 3f);
    public static Colour4 Cyan => Colour4.FromHSL(200 / 360f, 1f, 2 / 3f);
    public static Colour4 Blue => Colour4.FromHSL(240 / 360f, 1f, 2 / 3f);
    public static Colour4 Purple => Colour4.FromHSL(280 / 360f, 1f, 2 / 3f);
    public static Colour4 Pink => Colour4.FromHSL(320 / 360f, 1f, 2 / 3f);

    public static Colour4 ButtonRed => Colour4.FromHSL(0f, .5f, .3f);
    public static Colour4 ButtonGreen => Colour4.FromHSL(120 / 360f, .5f, .3f);

    public static Colour4 Footer1 => Colour4.FromHex("#EDBB98"); // ORANGE
    public static Colour4 Footer2 => Colour4.FromHex("#ED98A7"); // RED
    public static Colour4 Footer3 => Colour4.FromHex("#98CBED"); // CYAN
    public static Colour4 Footer4 => Colour4.FromHex("#B4ED98"); // GREEN
    public static Colour4 Footer5 => Colour4.FromHex("#9898ED"); // BLUE
    public static Colour4 Footer6 => Colour4.FromHex("#EDED98"); // YELLOW
    public static Colour4 Footer7 => Colour4.FromHex("#ED98D1"); // PINK
    public static Colour4 Footer8 => Colour4.FromHex("#98EDD1"); // MINT

    public static Colour4 TimingPoint => Colour4.FromHex("#00FF80");
    public static Colour4 ScrollVelocity => Colour4.FromHex("#00D4FF");
    public static Colour4 PreviewPoint => Colour4.FromHex("FDD27F");
    public static Colour4 LaneSwitch => Colour4.FromHex("#FF6666");
    public static Colour4 Flash => Colour4.FromHex("#FFCC66");
    public static Colour4 Pulse => Colour4.FromHex("#F0F975");
    public static Colour4 Shake => Colour4.FromHex("#01FEFE");
    public static Colour4 PlayfieldMove => Colour4.FromHex("#01FE55");
    public static Colour4 PlayfieldScale => Colour4.FromHex("#D279C4");
    public static Colour4 PlayfieldRotate => Colour4.FromHex("#8AF7A2");
    public static Colour4 HitObjectEase => Colour4.FromHex("#5B92FF");
    public static Colour4 LayerFade => Colour4.FromHex("#8AF3F7");
    public static Colour4 BeatPulse => Colour4.FromHex("#9973EF");
    public static Colour4 ScrollMultiply => Colour4.FromHex("#c73673");
    public static Colour4 TimeOffset => Colour4.FromHex("#fa8ca1");
    public static Colour4 Script => Colour4.FromHex("#c58a7b");
    public static Colour4 Note => Text;
    public static Colour4 Shader => Colour4.FromHex("#D65C5C");

    public static Colour4 Selection => Highlight;

    public static Colour4 DownloadFinished => Colour4.FromHex("#7BE87B");
    public static Colour4 DownloadQueued => Colour4.FromHex("#7BB1E8");

    public static Colour4 VoteUp => Colour4.FromHex("#43AFFC");
    public static Colour4 VoteDown => Colour4.FromHex("#FDC872");

    public static Colour4 SocialTwitter => Colour4.FromHex("#1da1f2");
    public static Colour4 SocialYoutube => Colour4.FromHex("#ff0000");
    public static Colour4 SocialTwitch => Colour4.FromHex("#6441a5");
    public static Colour4 SocialDiscord => Colour4.FromHex("#7289da");

    public static bool IsBright(Colour4 color)
    {
        var hsl = color.ToHSL();
        return hsl.Z >= .5f;
    }

    public static Colour4 DifficultyZero => Foreground;
    public static Colour4 Difficulty0 => Colour4.FromHex("#3355FF");
    public static Colour4 Difficulty5 => Colour4.FromHex("#3489FF");
    public static Colour4 Difficulty10 => Colour4.FromHex("#35BCFF");
    public static Colour4 Difficulty15 => Colour4.FromHex("#33FFDD");
    public static Colour4 Difficulty20 => Colour4.FromHex("#55FF33");
    public static Colour4 Difficulty25 => Colour4.FromHex("#FEFF33");
    public static Colour4 Difficulty30 => Colour4.FromHex("#FF3333");

    public static Colour4 GetDifficultyColor(float difficulty) => difficulty switch
    {
        <= 0 => DifficultyZero,
        <= 5 => ColourInfo.GradientHorizontal(Difficulty0, Difficulty5).Interpolate(new Vector2(difficulty / 5, 0)),
        <= 10 => ColourInfo.GradientHorizontal(Difficulty5, Difficulty10).Interpolate(new Vector2((difficulty - 5) / 5, 0)),
        <= 15 => ColourInfo.GradientHorizontal(Difficulty10, Difficulty15).Interpolate(new Vector2((difficulty - 10) / 5, 0)),
        <= 20 => ColourInfo.GradientHorizontal(Difficulty15, Difficulty20).Interpolate(new Vector2((difficulty - 15) / 5, 0)),
        <= 25 => ColourInfo.GradientHorizontal(Difficulty20, Difficulty25).Interpolate(new Vector2((difficulty - 20) / 5, 0)),
        <= 30 => ColourInfo.GradientHorizontal(Difficulty25, Difficulty30).Interpolate(new Vector2((difficulty - 25) / 5, 0)),
        _ => Difficulty30
    };

    public static Colour4 GetStatusColor(int status) => status switch
    {
        -2 => Colour4.FromHex("#8fffc8"),
        -1 => Colour4.FromHex("#888888"),
        0 => Colour4.FromHex("#888888"),
        1 => Colour4.FromHex("#f7b373"),
        2 => Colour4.FromHex("#ff7b74"),
        3 => Colour4.FromHex("#55b2ff"),
        _ => Colour4.Black
    };

    public static Colour4 GetKeyCountColor(int keyCount) => keyCount switch
    {
        1 => Colour4.FromHex("#333346"),
        2 => Colour4.FromHex("#a53541"),
        3 => Colour4.FromHex("#ff7a5a"),
        4 => Colour4.FromHex("#62bafe"),
        5 => Colour4.FromHex("#61f984"),
        6 => Colour4.FromHex("#e3bb45"),
        7 => Colour4.FromHex("#ec3b8d"),
        8 => Colour4.FromHex("#7ae9e9"),
        9 => Colour4.FromHex("#f7c5bb"),
        10 => Colour4.FromHex("#8c4451"),
        _ => Colour4.White
    };

    public static Colour4 GetLaneColor(int lane, int keyCount)
        => GetLaneColor(GetLaneColorIndex(lane, keyCount));

    public static Colour4 GetLaneColor(int index)
    {
        var colors = new[]
        {
            Colour4.White,
            Primary,
            Secondary,
            Primary.Lighten(.4f)
        };

        return colors[index];
    }

    public static int GetLaneColorIndex(int lane, int keyCount) => keyCount switch
    {
        1 => 3,
        2 => 2,
        3 => lane switch
        {
            1 or 3 => 2,
            _ => 3
        },
        4 => lane switch
        {
            1 or 4 => 1,
            _ => 2
        },
        5 => lane switch
        {
            1 or 5 => 1,
            2 or 4 => 2,
            _ => 3
        },
        6 => lane switch
        {
            1 or 3 or 4 or 6 => 2,
            _ => 1
        },
        7 => lane switch
        {
            2 or 6 => 1,
            1 or 3 or 5 or 7 => 2,
            _ => 3
        },
        8 => lane switch
        {
            2 or 7 => 1,
            1 or 3 or 6 or 8 => 2,
            _ => 3
        },
        9 => lane switch
        {
            1 or 3 or 7 or 9 => 1,
            2 or 4 or 6 or 8 => 2,
            _ => 3
        },
        10 => lane switch
        {
            1 or 3 or 8 or 10 => 1,
            2 or 4 or 7 or 9 => 2,
            _ => 3
        },
        _ => 0
    };

    public static Colour4 GetEditorSnapColor(int divisor) => divisor switch
    {
        1 => Colour4.White,
        2 => Colour4.FromHex("#FF5555"),
        3 => Colour4.FromHex("#8EFF55"),
        4 => Colour4.FromHex("#558EFF"),
        6 => Colour4.FromHex("#C655FF"),
        8 => Colour4.FromHex("#FFE355"),
        12 => Colour4.FromHex("#FF55AA"),
        16 => Colour4.FromHex("#BFBFBF"),
        _ => Colour4.White
    };

    public static Colour4 GetModTypeColor(ModType modType) => modType switch
    {
        ModType.Rate => Colour4.FromHex("#ffdb69"),
        ModType.DifficultyDecrease => Colour4.FromHex("#b2ff66"),
        ModType.DifficultyIncrease => Colour4.FromHex("#ff6666"),
        ModType.Automation => Colour4.FromHex("#66b3ff"),
        ModType.Misc => Colour4.FromHex("#8866ff"),
        _ => Colour4.FromHex("#cccccc")
    };
}
