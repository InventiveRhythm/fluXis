using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Map.Drawables;

public partial class DifficultyChip : CircularContainer
{
    public double Rating
    {
        get => rating;
        set
        {
            rating = value;

            if (IsLoaded)
            {
                box.Colour = FluXisColors.GetDifficultyColor((float)Rating);
                text.Text = Rating.ToStringInvariant("00.00");
            }
        }
    }

    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public float FontSize { get; set; } = 18;

    private double rating;

    private Box box;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        InternalChildren = new Drawable[]
        {
            box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.GetDifficultyColor((float)Rating)
            },
            text = new FluXisSpriteText
            {
                Text = Rating.ToStringInvariant("00.00"),
                FontSize = FontSize,
                Colour = Colour4.Black,
                Alpha = .75f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
