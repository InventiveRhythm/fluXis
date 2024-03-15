using System;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Game.Audio;

public partial class UISamples : Component
{
    [Resolved(CanBeNull = true)]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    private float pan => cursorOverlay?.RelativePosition.X ?? 0;

    private Sample hover;
    private Sample click;
    private Sample clickDisabled;
    private Sample dropdownOpen;
    private Sample dropdownClose;
    private Sample overlayOpen;
    private Sample overlayClose;

    private const float pitch_variation = 0.02f;
    private const float pan_strength = 0.75f;
    private const int debounce_time = 50;

    private double lastHoverTime;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        hover = samples.Get("UI/hover");
        click = samples.Get("UI/click");
        clickDisabled = samples.Get("UI/click-disabled");
        dropdownOpen = samples.Get("UI/dropdown-open");
        dropdownClose = samples.Get("UI/dropdown-close");
        overlayOpen = samples.Get("UI/Overlay/open");
        overlayClose = samples.Get("UI/Overlay/close");
    }

    public void PlayPanned(Sample sample, float pan, bool randomizePitch = false)
    {
        if (sample == null)
            return;

        pan = Math.Clamp(pan, 0, 1);

        var channel = sample.GetChannel();
        channel.Balance.Value = (pan * 2 - 1) * pan_strength;

        if (randomizePitch)
            channel.Frequency.Value = 1f - pitch_variation / 2f + RNG.NextDouble(pitch_variation);

        channel.Play();
    }

    public void Hover(float customPan = -1)
    {
        if (Time.Current - lastHoverTime < debounce_time)
            return;

        customPan = customPan >= 0 ? customPan : pan;
        PlayPanned(hover, customPan, true);
        lastHoverTime = Time.Current;
    }

    public void Click(bool disabled = false, float customPan = -1)
    {
        customPan = customPan >= 0 ? customPan : pan;
        PlayPanned(disabled ? clickDisabled : click, customPan);
    }

    public void Dropdown(bool close)
    {
        if (close)
            dropdownClose?.Play();
        else
            dropdownOpen?.Play();
    }

    public void Overlay(bool close)
    {
        if (close)
            overlayClose?.Play();
        else
            overlayOpen?.Play();
    }
}
