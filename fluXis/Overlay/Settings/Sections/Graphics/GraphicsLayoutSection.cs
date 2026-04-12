using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Overlay.Settings.Sections.Graphics;

public partial class GraphicsLayoutSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Layout;
    public override IconUsage Icon => FontAwesome6.Solid.WindowRestore;

    private SettingsGraphicsStrings strings => LocalizationStrings.Settings.Graphics;

    private ResolutionDropdown resolutionDropdown;

    [BackgroundDependencyLoader]
    private void load(GameHost host, FrameworkConfigManager frameworkConfig)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<WindowMode>
            {
                Label = strings.WindowMode,
                Items = host.Window.SupportedWindowModes,
                Bindable = frameworkConfig.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
            },
            resolutionDropdown = new ResolutionDropdown
            {
                Label = strings.FullscreenResolution,
                Items = new List<Size>(),
                Bindable = frameworkConfig.GetBindable<Size>(FrameworkSetting.SizeFullscreen)
            }
        });

        host.Window.CurrentDisplayBindable.BindValueChanged(v => Scheduler.ScheduleIfNeeded(() =>
        {
            var modes = v.NewValue.DisplayModes;
            var sizes = new List<Size> { new(9999, 9999) };

            foreach (var mode in modes)
            {
                var s = mode.Size;

                if (!sizes.Any(x => x.Width == s.Width && x.Height == s.Height))
                    sizes.Add(s);
            }

            resolutionDropdown.Items = sizes;
        }), true);
    }

    private partial class ResolutionDropdown : SettingsDropdown<Size>
    {
        protected override Dropdown<Size> CreateMenu() => new ResolutionDropdownMenu();

        private partial class ResolutionDropdownMenu : CustomDropdown
        {
            protected override LocalisableString GenerateItemText(Size item)
                => item.Width >= 9999 ? "Default" : $"{item.Width}x{item.Height}";
        }
    }
}
