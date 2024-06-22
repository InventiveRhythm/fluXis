using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableScoreRank : CompositeDrawable
{
    public ScoreRank Rank
    {
        get => rank;
        set
        {
            if (rank == value)
                return;

            rank = value;

            if (IsLoaded)
                redraw();
        }
    }

    public float FontSize { get; init; } = 64;
    public bool Shadow { get; init; } = true;
    public bool AlternateColor { get; init; }

    private ScoreRank rank = ScoreRank.X;

    [BackgroundDependencyLoader]
    private void load() => redraw();

    private void redraw()
    {
        ClearInternal();
        var color = GetColor(Rank);
        var str = Rank.ToString();

        switch (str.Length)
        {
            case 1:
                drawSingleLetter(str, color);
                break;

            case 2:
                drawDoubleLetter(str[0].ToString(), color);
                break;
        }
    }

    private void drawSingleLetter(string letter, Colour4 color)
    {
        var text = new FluXisSpriteText
        {
            Text = letter,
            Font = FluXisFont.YoureGone,
            FontSize = FontSize,
            Colour = AlternateColor ? FluXisColors.Background2 : color,
            Shadow = Shadow,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };

        InternalChild = text;
    }

    private void drawDoubleLetter(string letter, Colour4 color)
    {
        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = letter,
                Font = FluXisFont.YoureGone,
                FontSize = FontSize,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shadow = true,
                Margin = new MarginPadding { Left = 10 },
                Colour = AlternateColor ? FluXisColors.Background2 : color.Darken(.4f),
                Alpha = AlternateColor ? .8f : 1
            },
            new FluXisSpriteText
            {
                Text = letter,
                Font = FluXisFont.YoureGone,
                FontSize = FontSize,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shadow = true,
                Margin = new MarginPadding { Right = 10 },
                Colour = AlternateColor ? FluXisColors.Background2 : color
            }
        };
    }

    public static Colour4 GetColor(ScoreRank rank, bool darken = false)
    {
        var color = Colour4.White;

        switch (rank)
        {
            case ScoreRank.X:
                color = Colour4.FromHex("#a9a9a9");
                break;

            case ScoreRank.SS:
            case ScoreRank.S:
                color = Colour4.FromHex("#ffc14a");
                break;

            case ScoreRank.AA:
            case ScoreRank.A:
                color = Colour4.FromHex("#84ff70");
                break;

            case ScoreRank.B:
                color = Colour4.FromHex("#70d7ff");
                break;

            case ScoreRank.C:
                color = Colour4.FromHex("#ff70ff");
                break;

            case ScoreRank.D:
                color = Colour4.FromHex("#ff686b");
                break;
        }

        if (darken && rank is ScoreRank.S or ScoreRank.A)
            color = color.Darken(.4f);

        return color;
    }
}
