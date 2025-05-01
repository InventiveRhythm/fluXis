using System;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Map;
using fluXis.Overlay.Settings.UI;
using fluXis.Screens.Edit.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Input;

public partial class InputKeybindingsSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Keybindings;
    public override IconUsage Icon => FontAwesome6.Solid.Keyboard;

    private SettingsInputStrings strings => LocalizationStrings.Settings.Input;

    [Resolved]
    private MapStore mapStore { get; set; }

    private readonly BindableBool showOtherModes = new(true);

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AddRange(new Drawable[]
        {
            new SettingsSubSectionTitle(strings.Navigation),
            new SettingsKeybind
            {
                Label = strings.PreviousSelection,
                Keybinds = new object[] { FluXisGlobalKeybind.Previous }
            },
            new SettingsKeybind
            {
                Label = strings.NextSelection,
                Keybinds = new object[] { FluXisGlobalKeybind.Next }
            },
            new SettingsKeybind
            {
                Label = strings.PreviousGroup,
                Keybinds = new object[] { FluXisGlobalKeybind.PreviousGroup }
            },
            new SettingsKeybind
            {
                Label = strings.NextGroup,
                Keybinds = new object[] { FluXisGlobalKeybind.NextGroup }
            },
            new SettingsKeybind
            {
                Label = strings.Back,
                Keybinds = new object[] { FluXisGlobalKeybind.Back }
            },
            new SettingsKeybind
            {
                Label = strings.Select,
                Keybinds = new object[] { FluXisGlobalKeybind.Select }
            },
            new SettingsSubSectionTitle(strings.SongSelect),
            new SettingsKeybind
            {
                Label = strings.DecreaseRate,
                Keybinds = new object[] { FluXisGlobalKeybind.DecreaseRate }
            },
            new SettingsKeybind
            {
                Label = strings.IncreaseRate,
                Keybinds = new object[] { FluXisGlobalKeybind.IncreaseRate }
            },
            new SettingsSubSectionTitle(strings.Editing),
            new SettingsKeybind
            {
                Label = strings.DeleteSelection,
                Keybinds = new object[] { FluXisGlobalKeybind.Delete }
            },
            new SettingsKeybind
            {
                Label = strings.Undo,
                Keybinds = new object[] { EditorKeybinding.Undo }
            },
            new SettingsKeybind
            {
                Label = strings.Redo,
                Keybinds = new object[] { EditorKeybinding.Redo }
            },
            new SettingsDropdown<EditorScrollAction>
            {
                Label = "Scroll Action",
                Items = Enum.GetValues<EditorScrollAction>(),
                Bindable = config.GetBindable<EditorScrollAction>(FluXisSetting.EditorScrollAction),
                Padded = true
            },
            new SettingsDropdown<EditorScrollAction>
            {
                Label = "Ctrl-Scroll Action",
                Items = Enum.GetValues<EditorScrollAction>(),
                Bindable = config.GetBindable<EditorScrollAction>(FluXisSetting.EditorControlScrollAction),
                Padded = true
            },
            new SettingsDropdown<EditorScrollAction>
            {
                Label = "Shift-Scroll Action",
                Items = Enum.GetValues<EditorScrollAction>(),
                Bindable = config.GetBindable<EditorScrollAction>(FluXisSetting.EditorShiftScrollAction),
                Padded = true
            },
            new SettingsDropdown<EditorScrollAction>
            {
                Label = "Ctrl-Shift-Scroll Action",
                Items = Enum.GetValues<EditorScrollAction>(),
                Bindable = config.GetBindable<EditorScrollAction>(FluXisSetting.EditorControlShiftScrollAction),
                Padded = true
            },
            new SettingsSubSectionTitle(strings.Overlays),
            new SettingsKeybind
            {
                Label = strings.ToggleSettings,
                Keybinds = new object[] { FluXisGlobalKeybind.ToggleSettings }
            },
            new SettingsKeybind
            {
                Label = strings.ToggleDashboard,
                Keybinds = new object[] { FluXisGlobalKeybind.ToggleDashboard }
            },
            new SettingsSubSectionTitle(strings.Audio),
            new SettingsKeybind
            {
                Label = strings.VolumeDown,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeDecrease }
            },
            new SettingsKeybind
            {
                Label = strings.VolumeUp,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeIncrease }
            },
            new SettingsKeybind
            {
                Label = strings.PreviousVolumeCategory,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumePreviousCategory }
            },
            new SettingsKeybind
            {
                Label = strings.NextVolumeCategory,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeNextCategory }
            },
            new SettingsKeybind
            {
                Label = strings.PreviousTrack,
                Keybinds = new object[] { FluXisGlobalKeybind.MusicPrevious }
            },
            new SettingsKeybind
            {
                Label = strings.NextTrack,
                Keybinds = new object[] { FluXisGlobalKeybind.MusicNext }
            },
            new SettingsKeybind
            {
                Label = strings.PlayPause,
                Keybinds = new object[] { FluXisGlobalKeybind.MusicPause }
            },
            new SettingsSubSectionTitle(strings.Keymodes),
            new SettingsKeybind
            {
                Label = strings.FourKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key4k1,
                    FluXisGameplayKeybind.Key4k2,
                    FluXisGameplayKeybind.Key4k3,
                    FluXisGameplayKeybind.Key4k4
                }
            },
            new SettingsKeybind
            {
                Label = strings.FiveKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key5k1,
                    FluXisGameplayKeybind.Key5k2,
                    FluXisGameplayKeybind.Key5k3,
                    FluXisGameplayKeybind.Key5k4,
                    FluXisGameplayKeybind.Key5k5
                }
            },
            new SettingsKeybind
            {
                Label = strings.SixKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key6k1,
                    FluXisGameplayKeybind.Key6k2,
                    FluXisGameplayKeybind.Key6k3,
                    FluXisGameplayKeybind.Key6k4,
                    FluXisGameplayKeybind.Key6k5,
                    FluXisGameplayKeybind.Key6k6
                }
            },
            new SettingsKeybind
            {
                Label = strings.SevenKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key7k1,
                    FluXisGameplayKeybind.Key7k2,
                    FluXisGameplayKeybind.Key7k3,
                    FluXisGameplayKeybind.Key7k4,
                    FluXisGameplayKeybind.Key7k5,
                    FluXisGameplayKeybind.Key7k6,
                    FluXisGameplayKeybind.Key7k7
                }
            },
            new SettingsKeybind
            {
                Label = strings.EightKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key8k1,
                    FluXisGameplayKeybind.Key8k2,
                    FluXisGameplayKeybind.Key8k3,
                    FluXisGameplayKeybind.Key8k4,
                    FluXisGameplayKeybind.Key8k5,
                    FluXisGameplayKeybind.Key8k6,
                    FluXisGameplayKeybind.Key8k7,
                    FluXisGameplayKeybind.Key8k8
                }
            },
            new SettingsSubSectionTitle(strings.KeymodesDual),
            new SettingsKeybind
            {
                Label = strings.FourKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key4k1D,
                    FluXisGameplayKeybind.Key4k2D,
                    FluXisGameplayKeybind.Key4k3D,
                    FluXisGameplayKeybind.Key4k4D
                }
            },
            new SettingsKeybind
            {
                Label = strings.FiveKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key5k1D,
                    FluXisGameplayKeybind.Key5k2D,
                    FluXisGameplayKeybind.Key5k3D,
                    FluXisGameplayKeybind.Key5k4D,
                    FluXisGameplayKeybind.Key5k5D
                }
            },
            new SettingsKeybind
            {
                Label = strings.SixKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key6k1D,
                    FluXisGameplayKeybind.Key6k2D,
                    FluXisGameplayKeybind.Key6k3D,
                    FluXisGameplayKeybind.Key6k4D,
                    FluXisGameplayKeybind.Key6k5D,
                    FluXisGameplayKeybind.Key6k6D
                }
            },
            new SettingsKeybind
            {
                Label = strings.SevenKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key7k1D,
                    FluXisGameplayKeybind.Key7k2D,
                    FluXisGameplayKeybind.Key7k3D,
                    FluXisGameplayKeybind.Key7k4D,
                    FluXisGameplayKeybind.Key7k5D,
                    FluXisGameplayKeybind.Key7k6D,
                    FluXisGameplayKeybind.Key7k7D
                }
            },
            new SettingsKeybind
            {
                Label = strings.EightKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key8k1D,
                    FluXisGameplayKeybind.Key8k2D,
                    FluXisGameplayKeybind.Key8k3D,
                    FluXisGameplayKeybind.Key8k4D,
                    FluXisGameplayKeybind.Key8k5D,
                    FluXisGameplayKeybind.Key8k6D,
                    FluXisGameplayKeybind.Key8k7D,
                    FluXisGameplayKeybind.Key8k8D
                }
            },
            new SettingsSubSectionTitle(strings.OtherKeymodes) { Visible = showOtherModes },
            new SettingsKeybind
            {
                Label = strings.OneKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key1k1
                }
            },
            new SettingsKeybind
            {
                Label = strings.TwoKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key2k1,
                    FluXisGameplayKeybind.Key2k2
                }
            },
            new SettingsKeybind
            {
                Label = strings.ThreeKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key3k1,
                    FluXisGameplayKeybind.Key3k2,
                    FluXisGameplayKeybind.Key3k3
                }
            },
            new SettingsKeybind
            {
                Label = strings.NineKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key9k1,
                    FluXisGameplayKeybind.Key9k2,
                    FluXisGameplayKeybind.Key9k3,
                    FluXisGameplayKeybind.Key9k4,
                    FluXisGameplayKeybind.Key9k5,
                    FluXisGameplayKeybind.Key9k6,
                    FluXisGameplayKeybind.Key9k7,
                    FluXisGameplayKeybind.Key9k8,
                    FluXisGameplayKeybind.Key9k9
                }
            },
            new SettingsKeybind
            {
                Label = strings.TenKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key10k1,
                    FluXisGameplayKeybind.Key10k2,
                    FluXisGameplayKeybind.Key10k3,
                    FluXisGameplayKeybind.Key10k4,
                    FluXisGameplayKeybind.Key10k5,
                    FluXisGameplayKeybind.Key10k6,
                    FluXisGameplayKeybind.Key10k7,
                    FluXisGameplayKeybind.Key10k8,
                    FluXisGameplayKeybind.Key10k9,
                    FluXisGameplayKeybind.Key10k10
                }
            },
            new SettingsSubSectionTitle(strings.OtherKeymodesDual) { Visible = showOtherModes },
            new SettingsKeybind
            {
                Label = strings.OneKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key1k1D
                }
            },
            new SettingsKeybind
            {
                Label = strings.TwoKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key2k1D,
                    FluXisGameplayKeybind.Key2k2D
                }
            },
            new SettingsKeybind
            {
                Label = strings.ThreeKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key3k1D,
                    FluXisGameplayKeybind.Key3k2D,
                    FluXisGameplayKeybind.Key3k3D
                }
            },
            new SettingsKeybind
            {
                Label = strings.NineKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key9k1D,
                    FluXisGameplayKeybind.Key9k2D,
                    FluXisGameplayKeybind.Key9k3D,
                    FluXisGameplayKeybind.Key9k4D,
                    FluXisGameplayKeybind.Key9k5D,
                    FluXisGameplayKeybind.Key9k6D,
                    FluXisGameplayKeybind.Key9k7D,
                    FluXisGameplayKeybind.Key9k8D,
                    FluXisGameplayKeybind.Key9k9D
                }
            },
            new SettingsKeybind
            {
                Label = strings.TenKey,
                EnabledBindable = showOtherModes,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key10k1D,
                    FluXisGameplayKeybind.Key10k2D,
                    FluXisGameplayKeybind.Key10k3D,
                    FluXisGameplayKeybind.Key10k4D,
                    FluXisGameplayKeybind.Key10k5D,
                    FluXisGameplayKeybind.Key10k6D,
                    FluXisGameplayKeybind.Key10k7D,
                    FluXisGameplayKeybind.Key10k8D,
                    FluXisGameplayKeybind.Key10k9D,
                    FluXisGameplayKeybind.Key10k10D
                }
            },
            new SettingsSubSectionTitle(strings.InGame),
            new SettingsKeybind
            {
                Label = strings.SkipIntro,
                Keybinds = new object[] { FluXisGlobalKeybind.Skip }
            },
            new SettingsKeybind
            {
                Label = strings.DecreaseSpeed,
                Keybinds = new object[] { FluXisGlobalKeybind.ScrollSpeedDecrease }
            },
            new SettingsKeybind
            {
                Label = strings.IncreaseSpeed,
                Keybinds = new object[] { FluXisGlobalKeybind.ScrollSpeedIncrease }
            },
            new SettingsKeybind
            {
                Label = strings.QuickRestart,
                Keybinds = new object[] { FluXisGlobalKeybind.QuickRestart }
            },
            new SettingsKeybind
            {
                Label = strings.QuickExit,
                Keybinds = new object[] { FluXisGlobalKeybind.QuickExit }
            },
            new SettingsKeybind
            {
                Label = strings.SeekBackward,
                Keybinds = new object[] { FluXisGlobalKeybind.SeekBackward }
            },
            new SettingsKeybind
            {
                Label = strings.SeekForward,
                Keybinds = new object[] { FluXisGlobalKeybind.SeekForward }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        mapStore.CollectionUpdated += () => Schedule(updateOtherKeymodes);
        updateOtherKeymodes();
    }

    private void updateOtherKeymodes()
        => showOtherModes.Value = mapStore.MapSets.Any(x => x.Maps.Any(y => y.KeyCount is > 8 or < 4));
}
