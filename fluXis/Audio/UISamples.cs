using System;
using System.Diagnostics;
using fluXis.Configuration;
using fluXis.Overlay.Mouse;
using fluXis.Skinning;
using fluXis.Skinning.Default;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace fluXis.Audio;

public partial class UISamples : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private SkinManager skinManager { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ISkin skin { get; set; }

    private float pan => cursorOverlay?.RelativePosition.X ?? 0;

    private Sample back;
    private Sample select;
    private DebouncedSample hover;
    private PitchVariatedSample click;
    private Sample clickDisabled;
    private Sample dropdownOpen;
    private Sample dropdownClose;
    private Sample overlayOpen;
    private Sample overlayClose;
    private Sample panelOpen;
    private Sample panelDangerOpen;
    private Sample panelClose;
    private Sample panelDangerClose;
    private Sample skinSelectClick;

    private Bindable<double> panStrength;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, ISampleStore samples, FluXisConfig config)
    {
        panStrength = config.GetBindable<double>(FluXisSetting.UIPanning);

        var sk = skin ?? new DefaultSkin(textures, samples);

        back = sk.GetUISample(SampleType.Back);
        select = sk.GetUISample(SampleType.Select);
        AddInternal(hover = new DebouncedSample(new PitchVariatedSample(sk.GetUISample(SampleType.Hover))));
        AddInternal(click = new PitchVariatedSample(sk.GetUISample(SampleType.Click)));
        clickDisabled = sk.GetUISample(SampleType.ClickDisabled);
        skinSelectClick = sk.GetUISample(SampleType.SkinSelectClick);

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
        clickDisabled?.Dispose();
        dropdownOpen?.Dispose();
        dropdownClose?.Dispose();
        overlayOpen?.Dispose();
        overlayClose?.Dispose();
        panelOpen?.Dispose();
        panelDangerOpen?.Dispose();
        panelClose?.Dispose();
        panelDangerClose?.Dispose();
        skinSelectClick?.Dispose();
    }

    private void onSkinChanged()
    {
        back?.Dispose();
        select?.Dispose();
        clickDisabled?.Dispose();
        skinSelectClick?.Dispose();

        Debug.Assert(skin != null);

        back = skin.GetUISample(SampleType.Back);
        select = skin.GetUISample(SampleType.Select);
        hover.ReplaceSample(new PitchVariatedSample(skin.GetUISample(SampleType.Hover)));
        click.ReplaceSample(skin.GetUISample(SampleType.Click));
        clickDisabled = skin.GetUISample(SampleType.ClickDisabled);
        skinSelectClick = skin.GetUISample(SampleType.SkinSelectClick);
    }

    public void Back() => back?.Play();
    public void Select() => select?.Play();

    public void Hover(float customPan = -1)
    {
        customPan = customPan >= 0 ? customPan : pan;
        PlayPanned(hover, customPan);
    }

    public void Click(bool disabled = false, float customPan = -1)
    {
        customPan = customPan >= 0 ? customPan : pan;
        PlayPanned(disabled ? clickDisabled : click, customPan);
    }

    public void SkinSelectClick() => skinSelectClick?.Play();

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

    public void PlayPanned(ISample sample, float pan)
    {
        if (sample == null)
            return;

        var db = sample as DebouncedSample;
        if (db is { CanPlay: false }) return;

        pan = Math.Clamp(pan, 0, 1);

        var channel = sample.GetChannel();
        channel.Balance.Value = (pan * 2 - 1) * panStrength.Value;
        channel.Play();
        db?.UpdateLastPlayed();
    }

    public enum SampleType
    {
        Back,
        Select,
        Hover,
        Click,
        ClickDisabled,
        SkinSelectClick
    }
}
