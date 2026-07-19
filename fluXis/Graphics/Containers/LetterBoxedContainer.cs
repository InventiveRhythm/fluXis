using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace fluXis.Graphics.Containers;

public partial class LetterBoxedContainer : Container
{
    private readonly Container content;

    protected override Container<Drawable> Content => content;

    public LetterBoxedContainer(bool enabled = true)
    {
        RelativeSizeAxes = Axes.Both;

        if (enabled)
        {
            AddInternal(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black,
                Depth = float.MaxValue,
            });
        }

        AddInternal(content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = enabled ? FillMode.Fit : FillMode.Stretch,
            FillAspectRatio = 16 / 9f,
            Masking = true
        });
    }
}
