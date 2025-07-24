using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline;

public partial class TimelineIndicator : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(4, 30);
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;
        RelativePositionAxes = Axes.X;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientVertical(Theme.Primary, Theme.Secondary)
            },
            new Triangle
            {
                Size = new Vector2(8),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.BottomCentre,
                Colour = Theme.Primary,
                Rotation = 180
            },
            new Triangle
            {
                Size = new Vector2(8),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Colour = Theme.Secondary
            }
        };
    }
}
