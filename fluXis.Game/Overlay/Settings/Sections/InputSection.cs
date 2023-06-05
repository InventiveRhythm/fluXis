using fluXis.Game.Input;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class InputSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Keyboard;
    public override string Title => "Input";

    private Bindable<double> localSensitivity;
    private Bindable<double> handlerSensitivity;
    private Bindable<bool> relativeMode;

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        AddRange(new Drawable[]
        {
            new SettingsItem
            {
                Label = "Gameplay",
                LabelSize = 38
            },
            new SettingsKeybind
            {
                Label = "1 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key1k1
                }
            },
            new SettingsKeybind
            {
                Label = "2 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key2k1,
                    FluXisKeybind.Key2k2
                }
            },
            new SettingsKeybind
            {
                Label = "3 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key3k1,
                    FluXisKeybind.Key3k2,
                    FluXisKeybind.Key3k3
                }
            },
            new SettingsKeybind
            {
                Label = "4 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key4k1,
                    FluXisKeybind.Key4k2,
                    FluXisKeybind.Key4k3,
                    FluXisKeybind.Key4k4
                }
            },
            new SettingsKeybind
            {
                Label = "5 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key5k1,
                    FluXisKeybind.Key5k2,
                    FluXisKeybind.Key5k3,
                    FluXisKeybind.Key5k4,
                    FluXisKeybind.Key5k5
                }
            },
            new SettingsKeybind
            {
                Label = "6 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key6k1,
                    FluXisKeybind.Key6k2,
                    FluXisKeybind.Key6k3,
                    FluXisKeybind.Key6k4,
                    FluXisKeybind.Key6k5,
                    FluXisKeybind.Key6k6
                }
            },
            new SettingsKeybind
            {
                Label = "7 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key7k1,
                    FluXisKeybind.Key7k2,
                    FluXisKeybind.Key7k3,
                    FluXisKeybind.Key7k4,
                    FluXisKeybind.Key7k5,
                    FluXisKeybind.Key7k6,
                    FluXisKeybind.Key7k7
                }
            },
            new SettingsKeybind
            {
                Label = "8 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key8k1,
                    FluXisKeybind.Key8k2,
                    FluXisKeybind.Key8k3,
                    FluXisKeybind.Key8k4,
                    FluXisKeybind.Key8k5,
                    FluXisKeybind.Key8k6,
                    FluXisKeybind.Key8k7,
                    FluXisKeybind.Key8k8
                }
            },
            new SettingsKeybind
            {
                Label = "9 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key9k1,
                    FluXisKeybind.Key9k2,
                    FluXisKeybind.Key9k3,
                    FluXisKeybind.Key9k4,
                    FluXisKeybind.Key9k5,
                    FluXisKeybind.Key9k6,
                    FluXisKeybind.Key9k7,
                    FluXisKeybind.Key9k8,
                    FluXisKeybind.Key9k9
                }
            },
            new SettingsKeybind
            {
                Label = "10 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key10k1,
                    FluXisKeybind.Key10k2,
                    FluXisKeybind.Key10k3,
                    FluXisKeybind.Key10k4,
                    FluXisKeybind.Key10k5,
                    FluXisKeybind.Key10k6,
                    FluXisKeybind.Key10k7,
                    FluXisKeybind.Key10k8,
                    FluXisKeybind.Key10k9,
                    FluXisKeybind.Key10k10
                }
            },
            new SettingsKeybind
            {
                Label = "Quick Restart",
                Keybinds = new[] { FluXisKeybind.Restart }
            },
            new SettingsKeybind
            {
                Label = "Quick Exit",
                Keybinds = new[] { FluXisKeybind.Exit }
            }
        });

        foreach (var handler in host.AvailableInputHandlers)
        {
            switch (handler)
            {
                case MouseHandler mh:
                    relativeMode = mh.UseRelativeMode.GetBoundCopy();
                    handlerSensitivity = mh.Sensitivity.GetBoundCopy();
                    localSensitivity = handlerSensitivity.GetUnboundCopy();
                    AddRange(new Drawable[]
                    {
                        new SettingsItem
                        {
                            Label = "Mouse",
                            LabelSize = 38
                        },
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
                    break;
            }
        }
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
