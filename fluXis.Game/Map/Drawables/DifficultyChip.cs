using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Map.Drawables;

public partial class DifficultyChip : CircularContainer
{
    public float Rating
    {
        get => rating;
        set
        {
            rating = value;

            if (IsLoaded)
            {
                box.Colour = FluXisColors.GetDifficultyColor(Rating);
                text.Text = Rating.ToStringInvariant("00.00");
            }
        }
    }

    private float rating;

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
                Colour = FluXisColors.GetDifficultyColor(Rating)
            },
            text = new FluXisSpriteText
            {
                Text = Rating.ToStringInvariant("00.00"),
                FontSize = 14,
                Colour = Colour4.Black,
                Alpha = .75f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
