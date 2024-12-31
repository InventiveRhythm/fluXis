using System;
using fluXis.Configuration;
using fluXis.Overlay.Mouse;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Audio;

public partial class UISamples : Component
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private SkinManager skinManager { get; set; }

    private float pan => cursorOverlay?.RelativePosition.X ?? 0;

    private Sample back;
    private Sample select;
    private Sample hover;
    private Sample click;
    private Sample clickDisabled;
    private Sample dropdownOpen;
    private Sample dropdownClose;
    private Sample overlayOpen;
    private Sample overlayClose;
    private Sample panelOpen;
    private Sample panelDangerOpen;
    private Sample panelClose;
    private Sample panelDangerClose;

    private Bindable<double> panStrength;

    private const float pitch_variation = 0.02f;
    private const int debounce_time = 50;

    private double lastHoverTime;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        panStrength = config.GetBindable<double>(FluXisSetting.UIPanning);

        back = skinManager?.GetUISample(SampleType.Back);
        select = skinManager?.GetUISample(SampleType.Select);
        hover = skinManager?.GetUISample(SampleType.Hover);
        click = skinManager?.GetUISample(SampleType.Click);
        clickDisabled = skinManager?.GetUISample(SampleType.ClickDisabled);
        dropdownOpen = samples.Get("UI/dropdown-open");
        dropdownClose = samples.Get("UI/dropdown-close");
        overlayOpen = samples.Get("UI/Overlay/open");
        overlayClose = samples.Get("UI/Overlay/close");
        panelOpen = samples.Get("UI/panel-open");
        panelDangerOpen = samples.Get("UI/panel-open-danger");
        panelClose = samples.Get("UI/panel-close");
        panelDangerClose = samples.Get("UI/panel-close-danger");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (skinManager != null)
            skinManager.SkinChanged += onSkinChanged;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (skinManager != null)
            skinManager.SkinChanged -= onSkinChanged;

        back?.Dispose();
        select?.Dispose();
        hover?.Dispose();
        click?.Dispose();
        clickDisabled?.Dispose();
        dropdownOpen?.Dispose();
        dropdownClose?.Dispose();
        overlayOpen?.Dispose();
        overlayClose?.Dispose();
        panelOpen?.Dispose();
        panelDangerOpen?.Dispose();
        panelClose?.Dispose();
        panelDangerClose?.Dispose();
    }

    private void onSkinChanged()
    {
        back?.Dispose();
        select?.Dispose();
        hover?.Dispose();
        click?.Dispose();
        clickDisabled?.Dispose();

        // we're sure skinManager is not null here
        back = skinManager!.GetUISample(SampleType.Back);
        select = skinManager.GetUISample(SampleType.Select);
        hover = skinManager.GetUISample(SampleType.Hover);
        click = skinManager.GetUISample(SampleType.Click);
        clickDisabled = skinManager.GetUISample(SampleType.ClickDisabled);
    }

    public void Back() => back?.Play();
    public void Select() => select?.Play();

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

    public void PanelOpen(bool danger = false) => (danger ? panelDangerOpen : panelOpen)?.Play();
    public void PanelClose(bool danger = false) => (danger ? panelDangerClose : panelClose)?.Play();

    public void PlayPanned(Sample sample, float pan, bool randomizePitch = false)
    {
        if (sample == null)
            return;

        pan = Math.Clamp(pan, 0, 1);

        var channel = sample.GetChannel();
        channel.Balance.Value = (pan * 2 - 1) * panStrength.Value;

        if (randomizePitch)
            channel.Frequency.Value = 1f - pitch_variation / 2f + RNG.NextDouble(pitch_variation);

        channel.Play();
    }

    public enum SampleType
    {
        Back,
        Select,
        Hover,
        Click,
        ClickDisabled
    }
}
