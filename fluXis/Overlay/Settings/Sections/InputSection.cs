using fluXis.Graphics.Sprites.Icons;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Input;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Overlay.Settings.Sections;

#nullable enable

public partial class InputSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Keyboard;
    public override LocalisableString Title => LocalizationStrings.Settings.Input.Title;

    private InputKeybindingsSection bindings = null!;
    private InputGamepadSection gamepad = null!;

    private SettingsDivider? mouseDiv;
    private InputMouseSection? mouse;

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        Add(bindings = new InputKeybindingsSection());
        Add(gamepad = new InputGamepadSection());

        foreach (var handler in host.AvailableInputHandlers)
        {
            switch (handler)
            {
                case MouseHandler mh:
                    Add(mouseDiv = Divider);
                    Add(mouse = new InputMouseSection(mh));
                    break;
            }
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        GamepadHandler.OnGamepadStatusChanged += b =>
        {
            bindings.Visible.Value = !b;
            gamepad.Visible.Value = b;

            mouseDiv?.FadeTo(b ? 0f : 1f);
            if (mouse != null) mouse.Visible.Value = !b;
        };
    }
}
