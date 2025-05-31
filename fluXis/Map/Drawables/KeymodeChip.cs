using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Map.Drawables;

public partial class KeyCountChip : CircularContainer
{
    public int KeyCount
    {
        get => keycount;
        set
        {
            keycount = value;

            if (IsLoaded)
            {
                box.Colour = FluXisColors.GetKeyColor(KeyCount);
                text.Text = $"{KeyCount}K";
            }
        }
    }

    public float FontSize { get; set; } = 14;

    private int keycount;

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
                Colour = FluXisColors.GetKeyColor(KeyCount)
            },
            text = new FluXisSpriteText
            {
                Text = $"{KeyCount}K",
                FontSize = FontSize,
                Colour = Colour4.Black,
                Alpha = .75f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
