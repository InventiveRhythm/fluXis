using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics;

public partial class FluXisSpriteText : SpriteText
{
    public float FontSize
    {
        get => base.Font.Size;
        set => base.Font = base.Font.With(size: value);
    }

    public new FluXisFont Font
    {
        set => base.Font = base.Font.With(family: value.ToString());
    }

    public bool FixedWidth
    {
        get => base.Font.FixedWidth;
        set => base.Font = base.Font.With(fixedWidth: value);
    }

    public FluXisSpriteText()
    {
        Font = FluXisFont.Renogare;
    }

    public static FontUsage GetFont(FluXisFont font = FluXisFont.Renogare, float size = 20, bool fixedWidth = false) => new(font.ToString(), size, fixedWidth: fixedWidth);
}

public enum FluXisFont
{
    Renogare,
    YoureGone
}
