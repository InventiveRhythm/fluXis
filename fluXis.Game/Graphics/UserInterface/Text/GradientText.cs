using System;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;

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

    public bool Shadow { get; init; }

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

    protected override void LoadComplete()
    {
        base.LoadComplete();
        ScheduleAfterChildren(updateColors);
    }

    private void makeText()
    {
        Clear();

        foreach (var character in Text)
        {
            var sprite = new FluXisSpriteText()
            {
                Text = character.ToString(),
                FontSize = FontSize,
                Shadow = Shadow
            };

            Add(sprite);
        }

        ScheduleAfterChildren(updateColors);
    }

    private void updateColors()
    {
        // this is needed because DrawSize isn't right in the first frames
        var width = 0f;
        var height = 0f;
        var x = 0f;

        foreach (var drawable in this)
        {
            width += drawable.DrawRectangle.Width;
            height = Math.Max(height, drawable.DrawRectangle.Height);
        }

        foreach (var drawable in this)
        {
            var rect = drawable.DrawRectangle with
            {
                X = x,
                Y = drawable.Y
            };

            x += rect.Width;

            var col = Colour.Interpolate(rect / new Vector2(width, height));
            drawable.Colour = col;
        }
    }
}
