using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.UserInterface.Text;

/// <summary>
/// A container that displays text with a proper gradient.
/// </summary>
public partial class GradientText : FillFlowContainer
{
    public new ColourInfo Colour
    {
        get => colour;
        set
        {
            colour = value;

            if (IsLoaded)
                updateColors();
        }
    }

    public float WebFontSize
    {
        set => FontSize = FluXisSpriteText.GetWebFontSize(value);
    }

    public float FontSize { get; set; } = 20;

    public string Text
    {
        get => text;
        set
        {
            text = value;

            if (IsLoaded)
                makeText();
        }
    }

    private string text;
    private ColourInfo colour;

    public GradientText()
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Horizontal;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        makeText();
    }

    private void makeText()
    {
        Clear();

        foreach (var character in Text)
        {
            var sprite = new FluXisSpriteText()
            {
                Text = character.ToString(),
                FontSize = FontSize
            };

            Add(sprite);
        }

        Schedule(updateColors);
    }

    private void updateColors()
    {
        foreach (var drawable in this)
        {
            var rect = drawable.DrawRectangle with
            {
                X = drawable.X,
                Y = drawable.Y
            };

            var col = Colour.Interpolate(rect / DrawSize);
            drawable.Colour = col;
        }
    }
}
