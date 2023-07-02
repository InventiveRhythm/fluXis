using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace fluXis.Game.Graphics;

public static class FluXisColors
{
    public static Colour4 Accent => Colour4.FromHex("#3650eb");
    public static Colour4 Accent2 => Colour4.FromHex("#4846d5");
    public static Colour4 Accent3 => Colour4.FromHex("#533ec3");
    public static Colour4 Accent4 => Colour4.FromHex("#5f30a7");

    public static ColourInfo AccentGradient => ColourInfo.GradientHorizontal(Accent, Accent4);

    public static Colour4 Background => Colour4.FromHex("#1a1a20");
    public static Colour4 Background2 => Colour4.FromHex("#222228");

    public static Colour4 Hover => Colour4.FromHex("#7e7e7f");
    public static Colour4 Click => Colour4.FromHex("#ffffff");

    public static Colour4 Surface => Colour4.FromHex("#2a2a30");
    public static Colour4 Surface2 => Colour4.FromHex("#323238");
    public static Colour4 SurfaceDisabled => Colour4.FromHex("#2e2e34");

    public static Colour4 Text => Colour4.FromHex("#ffffff");
    public static Colour4 Text2 => Colour4.FromHex("#c8c8c8");
    public static Colour4 TextDisabled => Colour4.FromHex("#646464");
    public static Colour4 TextDark => Colour4.FromHex("#1a1a20");

    public static Colour4 RoleAdmin => Colour4.FromHex("#f7b373");
    public static Colour4 RoleMod => Colour4.FromHex("#73d173");
    public static Colour4 RolePurifier => Colour4.FromHex("#55b2ff");
    public static Colour4 RoleFeatured => Colour4.FromHex("#ff7b74");
    public static Colour4 RoleUser => Colour4.FromHex("#7455ff");
    public static Colour4 RoleBot => Colour4.FromHex("#1f1e33");

    public static Colour4 SocialTwitter => Colour4.FromHex("#1da1f2");
    public static Colour4 SocialYoutube => Colour4.FromHex("#ff0000");
    public static Colour4 SocialTwitch => Colour4.FromHex("#6441a5");
    public static Colour4 SocialDiscord => Colour4.FromHex("#7289da");

    public static bool IsBright(Colour4 color)
    {
        var hsl = color.ToHSL();
        return hsl.Z > .5f;
    }

    public static Colour4 GetRoleColor(int role)
    {
        return role switch
        {
            1 => RoleFeatured,
            2 => RolePurifier,
            3 => RoleMod,
            4 => RoleAdmin,
            5 => RoleBot,
            _ => RoleUser
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
    {
        return keyCount switch
        {
            1 => Accent.Lighten(.4f),
            2 => Accent4,
            3 => lane switch
            {
                1 or 3 => Accent4,
                _ => Accent.Lighten(.4f)
            },
            4 => lane switch
            {
                1 or 4 => Accent,
                _ => Accent4
            },
            5 => lane switch
            {
                1 or 5 => Accent,
                2 or 4 => Accent4,
                _ => Accent.Lighten(.4f)
            },
            6 => lane switch
            {
                1 or 3 or 4 or 6 => Accent4,
                _ => Accent
            },
            7 => lane switch
            {
                1 or 3 or 5 or 7 => Accent4,
                2 or 6 => Accent,
                _ => Accent.Lighten(.4f)
            },
            8 => lane switch
            {
                1 or 3 or 6 or 8 => Accent,
                _ => Accent4
            },
            9 => lane switch
            {
                1 or 3 or 7 or 9 => Accent,
                2 or 4 or 6 or 8 => Accent4,
                _ => Accent.Lighten(.4f)
            },
            10 => lane switch
            {
                1 or 10 => Accent.Lighten(.4f),
                3 or 5 or 6 or 8 => Accent4,
                _ => Accent
            },
            _ => Colour4.White
        };
    }
}
