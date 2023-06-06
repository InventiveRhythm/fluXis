using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Handlers.Mouse;

namespace fluXis.Game.Overlay.Settings.Sections.Input;

public partial class InputMouseSection : SettingsSubSection
{
    public override string Title => "Mouse";

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
                Label = "High Precision Mouse",
                Bindable = mh.UseRelativeMode.GetBoundCopy()
            },
            new SettingsSlider<double>
            {
                Label = "Sensitivity",
                Description = "This is only used when High Precision Mouse is enabled.",
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
