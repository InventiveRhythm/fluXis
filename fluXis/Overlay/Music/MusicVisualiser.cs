using fluXis.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Overlay.Music;

public partial class MusicVisualiser : Container
{
    [Resolved]
    private IAmplitudeProvider amplitudeProvider { get; set; }

    private const int bar_count = 128;
    private const int trim_bars = 12; // trims the very high ends of the bars where it is always near zero
    private const int active_bars = bar_count - trim_bars;
    private const float bar_width = 1f / active_bars;

    public bool Visible;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Blending = BlendingParameters.Additive;
        Height = 400;
        Alpha = 0.1f;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        for (int i = 0; i < active_bars; i++)
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

        if (!Visible) return;

        var amplitudes = amplitudeProvider.Amplitudes;
        if (amplitudes == null || amplitudes.Length == 0) return;

        int binsPerBar = amplitudes.Length / active_bars;

        for (var i = 0; i < active_bars; i++)
        {
            float sum = 0;
            float max = 0;
            int startBin = i * binsPerBar;

            for (int j = 0; j < binsPerBar && (startBin + j) < amplitudes.Length; j++)
            {
                float val = amplitudes[startBin + j];
                sum += val;
                if (val > max) max = val;
            }

            Children[i].Height = (sum / System.Math.Max(1, binsPerBar)) * 0.5f + max * 0.5f;
        }
    }
}
