using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultTopLaneCover : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        RelativePositionAxes = Axes.Both;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        Height = 0.5f;
        Colour = FluXisColors.Background3;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Height = 0.9f
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Height = 0.1f,
                Colour = ColourInfo.GradientVertical(Colour4.White, Colour4.White.Opacity(0))
            }
        };
    }
}
