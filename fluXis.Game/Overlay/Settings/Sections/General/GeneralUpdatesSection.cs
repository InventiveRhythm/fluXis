using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;

namespace fluXis.Game.Overlay.Settings.Sections.General;

public partial class GeneralUpdatesSection : SettingsSubSection
{
    public override string Title => "Updates";
    public override IconUsage Icon => FontAwesome6.Solid.Rotate;

    private InputManager inputManager;

    [BackgroundDependencyLoader]
    private void load(FluXisGameBase game)
    {
        Add(new SettingsButton
        {
            Label = "Check for updates",
            Description = "Checks for updates and downloads them if available.",
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
