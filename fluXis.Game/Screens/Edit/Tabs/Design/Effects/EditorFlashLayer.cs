using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Effects;

public partial class EditorFlashLayer : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public bool InBackground { get; init; } = false;

    private Box box;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChild = box = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0
        };
    }

    protected override void Update()
    {
        base.Update();

        var currentFlash = map.MapEvents.FlashEvents.LastOrDefault(e => e.Time <= clock.CurrentTime && e.InBackground == InBackground);

        if (currentFlash != null)
        {
            var progress = (float)(clock.CurrentTime - currentFlash.Time) / currentFlash.Duration;

            switch (progress)
            {
                case > 1:
                    box.Colour = currentFlash.EndColor;
                    box.Alpha = currentFlash.EndOpacity;
                    return;

                case <= 0:
                    box.Colour = currentFlash.StartColor;
                    box.Alpha = currentFlash.StartOpacity;
                    return;
            }

            var gradient = ColourInfo.GradientHorizontal(currentFlash.StartColor, currentFlash.EndColor);
            box.Colour = gradient.Interpolate(new Vector2((float)progress, 0));
            box.Alpha = Interpolation.ValueAt(progress, currentFlash.StartOpacity, currentFlash.EndOpacity, 0, 1);
        }
        else
        {
            box.Alpha = 0;
        }
    }
}
