using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Map;
using fluXis.Overlay.Settings.UI;
using fluXis.Overlay.Settings.UI.Keybind;
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
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSubSectionTitle(strings.Navigation),
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.PreviousSelection,
                Keybinds = new[] { FluXisGlobalKeybind.Previous }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.NextSelection,
                Keybinds = new[] { FluXisGlobalKeybind.Next }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.PreviousGroup,
                Keybinds = new[] { FluXisGlobalKeybind.PreviousGroup }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.NextGroup,
                Keybinds = new[] { FluXisGlobalKeybind.NextGroup }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.Back,
                Keybinds = new[] { FluXisGlobalKeybind.Back }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.Select,
                Keybinds = new[] { FluXisGlobalKeybind.Select }
            },
            new SettingsSubSectionTitle(strings.SongSelect),
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.DecreaseRate,
                Keybinds = new[] { FluXisGlobalKeybind.DecreaseRate }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.IncreaseRate,
                Keybinds = new[] { FluXisGlobalKeybind.IncreaseRate }
            },
            new SettingsSubSectionTitle(strings.Overlays),
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.ToggleSettings,
                Keybinds = new[] { FluXisGlobalKeybind.ToggleSettings }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.ToggleDashboard,
                Keybinds = new[] { FluXisGlobalKeybind.ToggleDashboard }
            },
            new SettingsSubSectionTitle(strings.Audio),
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.VolumeDown,
                Keybinds = new[] { FluXisGlobalKeybind.VolumeDecrease }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.VolumeUp,
                Keybinds = new[] { FluXisGlobalKeybind.VolumeIncrease }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.PreviousVolumeCategory,
                Keybinds = new[] { FluXisGlobalKeybind.VolumePreviousCategory }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.NextVolumeCategory,
                Keybinds = new[] { FluXisGlobalKeybind.VolumeNextCategory }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.PreviousTrack,
                Keybinds = new[] { FluXisGlobalKeybind.MusicPrevious }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.NextTrack,
                Keybinds = new[] { FluXisGlobalKeybind.MusicNext }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.PlayPause,
                Keybinds = new[] { FluXisGlobalKeybind.MusicPause }
            },
            new SettingsSubSectionTitle(strings.Keymodes),
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.FourKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key4k1,
                    FluXisGameplayKeybind.Key4k2,
                    FluXisGameplayKeybind.Key4k3,
                    FluXisGameplayKeybind.Key4k4
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.FiveKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key5k1,
                    FluXisGameplayKeybind.Key5k2,
                    FluXisGameplayKeybind.Key5k3,
                    FluXisGameplayKeybind.Key5k4,
                    FluXisGameplayKeybind.Key5k5
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.SixKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key6k1,
                    FluXisGameplayKeybind.Key6k2,
                    FluXisGameplayKeybind.Key6k3,
                    FluXisGameplayKeybind.Key6k4,
                    FluXisGameplayKeybind.Key6k5,
                    FluXisGameplayKeybind.Key6k6
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.SevenKey,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.EightKey,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.FourKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key4k1D,
                    FluXisGameplayKeybind.Key4k2D,
                    FluXisGameplayKeybind.Key4k3D,
                    FluXisGameplayKeybind.Key4k4D
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.FiveKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key5k1D,
                    FluXisGameplayKeybind.Key5k2D,
                    FluXisGameplayKeybind.Key5k3D,
                    FluXisGameplayKeybind.Key5k4D,
                    FluXisGameplayKeybind.Key5k5D
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.SixKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key6k1D,
                    FluXisGameplayKeybind.Key6k2D,
                    FluXisGameplayKeybind.Key6k3D,
                    FluXisGameplayKeybind.Key6k4D,
                    FluXisGameplayKeybind.Key6k5D,
                    FluXisGameplayKeybind.Key6k6D
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.SevenKey,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.EightKey,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.OneKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key1k1
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.TwoKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key2k1,
                    FluXisGameplayKeybind.Key2k2
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.ThreeKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key3k1,
                    FluXisGameplayKeybind.Key3k2,
                    FluXisGameplayKeybind.Key3k3
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.NineKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.TenKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.OneKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key1k1D
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.TwoKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key2k1D,
                    FluXisGameplayKeybind.Key2k2D
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.ThreeKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key3k1D,
                    FluXisGameplayKeybind.Key3k2D,
                    FluXisGameplayKeybind.Key3k3D
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.NineKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.TenKey,
                EnabledBindable = showOtherModes,
                Keybinds = new[]
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
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.SkipIntro,
                Keybinds = new[] { FluXisGlobalKeybind.Skip }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.QuickRestart,
                Keybinds = new[] { FluXisGlobalKeybind.QuickRestart }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.QuickExit,
                Keybinds = new[] { FluXisGlobalKeybind.QuickExit }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.SeekBackward,
                Keybinds = new[] { FluXisGlobalKeybind.SeekBackward }
            },
            new SettingsRealmKeybind<FluXisGlobalKeybind>
            {
                Label = strings.SeekForward,
                Keybinds = new[] { FluXisGlobalKeybind.SeekForward }
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
