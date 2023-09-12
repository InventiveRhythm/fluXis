using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring.Enums;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableScoreRank : Container
{
    public ScoreRank Rank
    {
        get => rank;
        set
        {
            rank = value;
            ClearInternal();
            drawLetters();
        }
    }

    public new float Size
    {
        get => size;
        set
        {
            size = value;
            Height = size;
            Width = size;
            ClearInternal();
            drawLetters();
        }
    }

    private ScoreRank rank = ScoreRank.X;
    private float size = 64;

    private void drawLetters()
    {
        switch (rank)
        {
            case ScoreRank.X:
                drawSingleLetter("X", Colour4.FromHex("#a9a9a9"));
                break;

            case ScoreRank.SS:
                drawDoubleLetter("S", Colour4.FromHex("#ffc14a"));
                break;

            case ScoreRank.S:
                drawSingleLetter("S", Colour4.FromHex("#ffc14a"));
                break;

            case ScoreRank.AA:
                drawDoubleLetter("A", Colour4.FromHex("#84ff70"));
                break;

            case ScoreRank.A:
                drawSingleLetter("A", Colour4.FromHex("#84ff70"));
                break;

            case ScoreRank.B:
                drawSingleLetter("B", Colour4.FromHex("#70d7ff"));
                break;

            case ScoreRank.C:
                drawSingleLetter("C", Colour4.FromHex("#ff70ff"));
                break;

            case ScoreRank.D:
                drawSingleLetter("D", Colour4.FromHex("#ff686b"));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void drawSingleLetter(string letter, Colour4 color)
    {
        Add(new FluXisSpriteText
        {
            Text = letter,
            Font = FluXisFont.YoureGone,
            FontSize = Height,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Shadow = true,
            Colour = color
        });
    }

    private void drawDoubleLetter(string letter, Colour4 color)
    {
        AddRange(new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = letter,
                Font = FluXisFont.YoureGone,
                FontSize = Height,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shadow = true,
                Margin = new MarginPadding { Left = 10 },
                Colour = color.Darken(.4f)
            },
            new FluXisSpriteText
            {
                Text = letter,
                Font = FluXisFont.YoureGone,
                FontSize = Height,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shadow = true,
                Margin = new MarginPadding { Right = 10 },
                Colour = color
            }
        });
    }
}
