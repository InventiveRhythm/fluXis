using System;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Graphics.Sprites;

public partial class FluXisSpriteText : SpriteText
{
    private const float web_scale = 1.4f;

    public float FontSize
    {
        get => base.Font.Size;
        set => base.Font = base.Font.With(size: value);
    }

    // this is super wacky...
    // the font is rendered smaller here than in a web browser/figma
    // this multiplier should be about right to make them match up
    public float WebFontSize { set => FontSize = value * web_scale; }

    public new FluXisFont Font
    {
        set => base.Font = base.Font.With(family: value.ToString());
    }

    public bool FixedWidth
    {
        get => base.Font.FixedWidth;
        set => base.Font = base.Font.With(fixedWidth: value);
    }

    [Obsolete("Use TruncatingText instead.")]
    public new bool Truncate
    {
        set => throw new InvalidOperationException("Use TruncatingText instead.");
    }

    public FluXisSpriteText()
    {
        Colour = FluXisColors.Text;
        Font = FluXisFont.RenogareSoft;
    }

    public static FontUsage GetFont(FluXisFont font = FluXisFont.RenogareSoft, float size = 20, bool fixedWidth = false) => new(font.ToString(), size, fixedWidth: fixedWidth);

    public static float GetWebFontSize(float i) => i * web_scale;
}

// ReSharper disable IdentifierTypo
public enum FluXisFont
{
    Renogare,
    RenogareSoft,
    YoureGone
}
