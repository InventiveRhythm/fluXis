using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Input;

public partial class InputMouseSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Mouse;
    public override IconUsage Icon => FontAwesome6.Solid.ComputerMouse;

    private SettingsInputStrings strings => LocalizationStrings.Settings.Input;

    private MouseHandler mh { get; }
    private Bindable<double> localSensitivity { get; }
    private Bindable<double> handlerSensitivity { get; }
    private Bindable<bool> relativeMode { get; }

    public InputMouseSection(MouseHandler mouseHandler)
    {
        mh = mouseHandler;
        relativeMode = mh.UseRelativeMode.GetBoundCopy();
        handlerSensitivity = mh.Sensitivity.GetBoundCopy();
        localSensitivity = handlerSensitivity.GetUnboundCopy();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.HighPrecisionMouse,
                Description = strings.HighPrecisionMouseDescription,
                Bindable = mh.UseRelativeMode.GetBoundCopy()
            },
            new SettingsSlider<double>
            {
                Label = strings.Sensitivity,
                Description = strings.SensitivityDescription,
                Bindable = mh.Sensitivity.GetBoundCopy(),
                Step = 0.01f
            }
        });
    }

    protected override void LoadComplete()
    {
        relativeMode.BindValueChanged(relative => localSensitivity.Disabled = !relative.NewValue, true);

        handlerSensitivity.BindValueChanged(val =>
        {
            bool disabled = localSensitivity.Disabled;

            localSensitivity.Disabled = false;
            localSensitivity.Value = val.NewValue;
            localSensitivity.Disabled = disabled;
        }, true);

        localSensitivity.BindValueChanged(val => handlerSensitivity.Value = val.NewValue);

        base.LoadComplete();
    }
}
