using fluXis.Game.Mods;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Color;

public static class FluXisColors
{
    public static Colour4 Accent => Colour4.FromHex("#3650eb");
    public static Colour4 Accent2 => Colour4.FromHex("#4846d5");
    public static Colour4 Accent3 => Colour4.FromHex("#533ec3");
    public static Colour4 Accent4 => Colour4.FromHex("#5f30a7");

    public static ColourInfo AccentGradient => ColourInfo.GradientHorizontal(Accent, Accent4);

    public static Colour4 Background1 => GetThemeColor(.1f, .1f);
    public static Colour4 Background2 => GetThemeColor(.1f, .15f);
    public static Colour4 Background3 => GetThemeColor(.1f, .2f);
    public static Colour4 Background4 => GetThemeColor(.1f, .25f);
    public static Colour4 Background5 => GetThemeColor(.1f, .3f);
    public static Colour4 Background6 => GetThemeColor(.1f, .35f);
    public static Colour4 Foreground => GetThemeColor(.1f, .6f);
    public static Colour4 Highlight => GetThemeColor(1f, .7f);
    public static Colour4 Link => GetThemeColor(1f, .8f);

    public static Colour4 GetThemeColor(float saturation, float lightness) => Colour4.FromHSL(240 / 360f, saturation, lightness);

    public static Colour4 Text => Colour4.FromHex("#ffffff");
    public static Colour4 Text2 => Colour4.FromHex("#cccccc");
    public static Colour4 TextDisabled => Colour4.FromHex("#646464");
    public static Colour4 TextDark => Background3;

    public static Colour4 ButtonRed => Colour4.FromHSL(0f, .5f, .3f);
    public static Colour4 Red => Colour4.FromHSL(0f, 1f, .67f);
    public static Colour4 ButtonGreen => Colour4.FromHSL(120 / 360f, .5f, .3f);
    public static Colour4 Green => Colour4.FromHSL(120 / 360f, 1f, .67f);

    public static Colour4 TimingPoint => Colour4.FromHex("#00FF80");
    public static Colour4 ScrollVelocity => Colour4.FromHex("#00D4FF");
    public static Colour4 PreviewPoint => Colour4.FromHex("FDD27F");
    public static Colour4 LaneSwitch => Colour4.FromHex("#FF6666");
    public static Colour4 Flash => Colour4.FromHex("#FFCC66");
    public static Colour4 Shake => Colour4.FromHex("#01FEFE");
    public static Colour4 PlayfieldMove => Colour4.FromHex("#01FE55");
    public static Colour4 PlayfieldScale => Colour4.FromHex("#D279C4");
    public static Colour4 PlayfieldRotate => Colour4.FromHex("#8AF7A2");
    public static Colour4 PlayfieldFade => Colour4.FromHex("#0180FE");
    public static Colour4 HitObjectFade => Colour4.FromHex("#8AF3F7");
    public static Colour4 BeatPulse => Colour4.FromHex("#FF6666");
    public static Colour4 Note => Colour4.FromHex("#FFFFFF");
    public static Colour4 Shader => Colour4.FromHex("#D65C5C");

    public static Colour4 Selection => Highlight;

    public static Colour4 DownloadFinished => Colour4.FromHex("#7BE87B");
    public static Colour4 DownloadQueued => Colour4.FromHex("#7BB1E8");

    public static Colour4 SocialTwitter => Colour4.FromHex("#1da1f2");
    public static Colour4 SocialYoutube => Colour4.FromHex("#ff0000");
    public static Colour4 SocialTwitch => Colour4.FromHex("#6441a5");
    public static Colour4 SocialDiscord => Colour4.FromHex("#7289da");

    public static bool IsBright(Colour4 color)
    {
        var hsl = color.ToHSL();
        return hsl.Z >= .5f;
    }

    public static Colour4 DifficultyZero => Colour4.FromHex("#888888");
    public static Colour4 Difficulty0 => Colour4.FromHex("#3355FF");
    public static Colour4 Difficulty5 => Colour4.FromHex("#3489FF");
    public static Colour4 Difficulty10 => Colour4.FromHex("#35BCFF");
    public static Colour4 Difficulty15 => Colour4.FromHex("#33FFDD");
    public static Colour4 Difficulty20 => Colour4.FromHex("#55FF33");
    public static Colour4 Difficulty25 => Colour4.FromHex("#FEFF33");
    public static Colour4 Difficulty30 => Colour4.FromHex("#FF3333");

    public static Colour4 GetDifficultyColor(float difficulty)
    {
        return difficulty switch
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
    }

    public static Colour4 GetStatusColor(int status)
    {
        return status switch
        {
            -2 => Colour4.FromHex("#8fffc8"),
            -1 => Colour4.FromHex("#888888"),
            0 => Colour4.FromHex("#888888"),
            1 => Colour4.FromHex("#f7b373"),
            2 => Colour4.FromHex("#ff7b74"),
            3 => Colour4.FromHex("#55b2ff"),
            _ => Colour4.Black
        };
    }

    public static Colour4 GetKeyColor(int keyCount)
    {
        return keyCount switch
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
    }

    public static Colour4 GetLaneColor(int lane, int keyCount)
        => GetLaneColor(GetLaneColorIndex(lane, keyCount));

    public static Colour4 GetLaneColor(int index)
    {
        var colors = new[]
        {
            Colour4.White,
            Accent,
            Accent4,
            Accent.Lighten(.4f)
        };

        return colors[index];
    }

    public static int GetLaneColorIndex(int lane, int keyCount)
    {
        return keyCount switch
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
                1 or 3 or 5 or 7 => 2,
                2 or 6 => 1,
                _ => 3
            },
            8 => lane switch
            {
                1 or 3 or 6 or 8 => 1,
                _ => 2
            },
            9 => lane switch
            {
                1 or 3 or 7 or 9 => 1,
                2 or 4 or 6 or 8 => 2,
                _ => 3
            },
            10 => lane switch
            {
                1 or 10 => 3,
                3 or 5 or 6 or 8 => 2,
                _ => 1
            },
            _ => 0
        };
    }

    public static Colour4 GetSnapColor(int divisor)
    {
        switch (divisor)
        {
            case 1:
                return Colour4.FromHex("#FF5555");

            case 2:
                return Colour4.FromHex("#558EFF");

            case 3:
                return Colour4.FromHex("#8EFF55");

            case 4:
                return Colour4.FromHex("#FFE355");

            case 6:
                return Colour4.FromHex("#C655FF");

            case 8:
                return Colour4.FromHex("#55FFAA");

            case 12:
                return Colour4.FromHex("#FF55AA");

            default:
                return Colour4.FromHex("#BFBFBF");
        }
    }

    public static Colour4 GetEditorSnapColor(int divisor, int val, int i)
    {
        switch (divisor)
        {
            case 1:
                return Colour4.White;

            case 2:
                return val == 0 ? Colour4.White : Colour4.Red;

            case 4:
                return val switch
                {
                    0 or 4 => Colour4.White,
                    1 or 3 => Colour4.FromHex("#0085ff"),
                    _ => Colour4.Red
                };

            case 3:
            case 6:
            case 12:
                if (val % 3 == 0) return Colour4.Red;

                return val == 0 ? Colour4.White : Colour4.Purple;

            case 8:
            case 16:
                if (val == 0) return Colour4.White;
                if ((i - 1) % 2 == 0) return Colour4.Gold;

                return i % 4 == 0 ? Colour4.Red : Colour4.FromHex("#0085ff");

            default:
                return val != 0 ? Colour4.FromHex(i % 2 == 0 ? "#af4fb8" : "#4e94b7") : Colour4.White;
        }
    }

    public static Colour4 GetModTypeColor(ModType modType)
    {
        switch (modType)
        {
            case ModType.Rate:
                return Colour4.FromHex("#ffdb69");

            case ModType.DifficultyDecrease:
                return Colour4.FromHex("#b2ff66");

            case ModType.DifficultyIncrease:
                return Colour4.FromHex("#ff6666");

            case ModType.Automation:
                return Colour4.FromHex("#66b3ff");

            case ModType.Misc:
                return Colour4.FromHex("#8866ff");

            default:
                return Colour4.FromHex("#cccccc");
        }
    }
}
