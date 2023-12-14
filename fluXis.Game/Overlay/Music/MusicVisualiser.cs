using fluXis.Game.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Music;

public partial class MusicVisualiser : Container
{
    [Resolved]
    private GlobalClock clock { get; set; }

    private const int bar_count = 128;
    private const float bar_width = 1f / bar_count;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 400;
        Alpha = 0.5f;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        for (int i = 0; i < bar_count; i++)
        {
            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                X = i * bar_width,
                Width = bar_width
            });
        }
    }

    protected override void Update()
    {
        base.Update();

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            var height = clock.Amplitudes[i];
            child.Height = height;
        }
    }
}
