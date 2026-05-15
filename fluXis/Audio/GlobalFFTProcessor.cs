using fluXis.Audio.FFT;
using fluXis.Audio.FFT.Structures.Processor;
using fluXis.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using System;
using osu.Framework.Bindables;
using osu.Framework.Utils;

namespace fluXis.Audio;

public partial class GlobalFFTProcessor : Component, IAmplitudeProvider
{
    [Resolved]
    private GlobalClock clock { get; set; }

    private static readonly FFTProcessor processor = new();
    public FFTParameters Parameters { get; private set; }

    public float[] Amplitudes { get; } = new float[256];
    private readonly float[] downwardVelocities = new float[256];

    public BindableBool Enabled = new(true);

    private const float noise_floor = 0.01f;
    private const float gravity = 20f;
    private const float popup_speed = 35f;

    private double lastAmplitudeUpdate;

    public GlobalFFTProcessor(FFTParameters? parameters = null)
    {
        Parameters = parameters ?? Styling.GlobalReactiveFFT;
    }

    protected override void Update()
    {
        base.Update();
        if (Enabled.Value) updateAmplitudes();
    }

    private void updateAmplitudes()
    {
        if (clock.CurrentTrack == null) return;

        const int update_fps = 120;

        if (Time.Current - lastAmplitudeUpdate < 1000f / update_fps)
            return;

        var elapsed = Time.Current - lastAmplitudeUpdate;
        lastAmplitudeUpdate = Time.Current;

        float delta = (float)elapsed / 1000f;
        var raw = processor.Process(clock.Amplitudes);

        for (var i = 0; i < raw.Length; i++)
        {
            float target = raw[i] < noise_floor ? 0 : raw[i];

            if (target > Amplitudes[i])
            {
                Amplitudes[i] = (float)Interpolation.Lerp(Amplitudes[i], target, Math.Clamp(delta * popup_speed, 0, 1));
                downwardVelocities[i] = 0;
            }
            else
            {
                downwardVelocities[i] += gravity * delta;
                Amplitudes[i] -= downwardVelocities[i] * delta;

                if (Amplitudes[i] < target)
                {
                    Amplitudes[i] = target;
                    downwardVelocities[i] = 0;
                }
            }

            Amplitudes[i] = Math.Max(0f, Amplitudes[i]);
        }
    }
}
