using System;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics;

public partial class DrawableGrade : Container
{
    public Grade Grade
    {
        get => grade;
        set
        {
            grade = value;
            ClearInternal();
            drawGrade();
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
            drawGrade();
        }
    }

    private Grade grade = Grade.X;
    private float size = 64;

    private void drawGrade()
    {
        switch (grade)
        {
            case Grade.X:
                drawSingleLetter("X", Colour4.FromHex("#a9a9a9"));
                break;

            case Grade.SS:
                drawDoubleLetter("S", Colour4.FromHex("#ffc14a"));
                break;

            case Grade.S:
                drawSingleLetter("S", Colour4.FromHex("#ffc14a"));
                break;

            case Grade.AA:
                drawDoubleLetter("A", Colour4.FromHex("#84ff70"));
                break;

            case Grade.A:
                drawSingleLetter("A", Colour4.FromHex("#84ff70"));
                break;

            case Grade.B:
                drawSingleLetter("B", Colour4.FromHex("#70d7ff"));
                break;

            case Grade.C:
                drawSingleLetter("C", Colour4.FromHex("#ff70ff"));
                break;

            case Grade.D:
                drawSingleLetter("D", Colour4.FromHex("#ff686b"));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void drawSingleLetter(string letter, Colour4 color)
    {
        Add(new SpriteText
        {
            Text = letter,
            Font = new FontUsage("Grade", Height),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Colour = color
        });
    }

    private void drawDoubleLetter(string letter, Colour4 color)
    {
        AddRange(new Drawable[]
        {
            new SpriteText
            {
                Text = letter,
                Font = new FontUsage("Grade", Height),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Left = 10 },
                Colour = color.Darken(.4f)
            },
            new SpriteText
            {
                Text = letter,
                Font = new FontUsage("Grade", Height),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Right = 10 },
                Colour = color
            }
        });
    }
}
