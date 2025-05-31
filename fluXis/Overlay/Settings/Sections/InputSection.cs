using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Overlay.Settings.Sections;

public partial class InputSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Keyboard;
    public override LocalisableString Title => LocalizationStrings.Settings.Input.Title;

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        Add(new InputKeybindingsSection());

        foreach (var handler in host.AvailableInputHandlers)
        {
            switch (handler)
            {
                case MouseHandler mh:
                    Add(Divider);
                    Add(new InputMouseSection(mh));
                    break;
            }
        }
    }
}
