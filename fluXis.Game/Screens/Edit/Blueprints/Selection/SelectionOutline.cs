using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Blueprints.Selection;

public partial class SelectionOutline : Container
{
    private int targetX { get; set; }
    private int targetY { get; set; }
    private int targetWidth { get; set; }
    private int targetHeight { get; set; }

    private const float duration = 200;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 10;
        BorderThickness = 5;
        BorderColour = FluXisColors.Selection;
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Selection,
            Alpha = 0.2f
        };
    }

    public void UpdatePositionAndSize(Vector2 pos, Vector2 size, bool instant = false)
    {
        int dur = instant ? 0 : (int)duration;

        if (targetX != pos.X)
        {
            targetX = (int)pos.X;
            this.MoveToX(targetX, dur, Easing.OutQuint);
        }

        if (targetY != pos.Y)
        {
            targetY = (int)pos.Y;
            this.MoveToY(targetY, dur, Easing.OutQuint);
        }

        if (targetWidth != size.X)
        {
            targetWidth = (int)size.X;
            this.ResizeWidthTo(targetWidth, dur, Easing.OutQuint);
        }

        if (targetHeight != size.Y)
        {
            targetHeight = (int)size.Y;
            this.ResizeHeightTo(targetHeight, dur, Easing.OutQuint);
        }
    }
}
