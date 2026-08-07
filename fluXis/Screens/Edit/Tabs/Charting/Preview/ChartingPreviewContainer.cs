using fluXis.Graphics;
using fluXis.Graphics.Containers;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Preview;

public partial class ChartingPreviewContainer : CompositeDrawable
{
    private Box dim;
    private ClickableContainer wrapper;
    private bool full;

    [UsedImplicitly]
    // ReSharper disable once RedundantDefaultMemberInitializer
    private float fullProgress = 0f;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren =
        [
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0f
            },
            wrapper = new ClickableContainer
            {
                Action = toggle,
                Padding = new MarginPadding(12),
                Child = new AspectRatioContainer(true)
                {
                    CornerRadius = 12,
                    Masking = true,
                    Child = new ChartingPreview()
                }
            }
        ];
    }

    protected override void Update()
    {
        base.Update();
        updateSize();

        return;

        void updateSize()
        {
            var min = new Vector2(384, 216) + new Vector2(Padding.TotalHorizontal, Padding.TotalVertical);
            var max = Parent!.DrawSize;

            var delta = max - min;
            wrapper.Size = min + delta * fullProgress;
            dim.Alpha = fullProgress * 0.9f;
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.F && !e.Repeat)
        {
            toggle();
            return true;
        }

        return base.OnKeyDown(e);
    }

    private void toggle()
    {
        full = !full;
        this.TransformTo(nameof(fullProgress), full ? 1f : 0f, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }
}
