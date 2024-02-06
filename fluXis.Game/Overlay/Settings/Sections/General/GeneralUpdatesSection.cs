using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.General;

public partial class GeneralUpdatesSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Updates;
    public override IconUsage Icon => FontAwesome6.Solid.Rotate;

    private SettingsGeneralStrings strings => LocalizationStrings.Settings.General;

    private InputManager inputManager;

    [BackgroundDependencyLoader]
    private void load(FluXisGameBase game)
    {
        Add(new SettingsButton
        {
            Label = strings.UpdatesCheck,
            Description = strings.UpdatesCheckDescription,
            ButtonText = "Check",
            Action = () => game.PerformUpdateCheck(false, inputManager.CurrentState.Keyboard.AltPressed)
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = GetContainingInputManager();
    }
}
