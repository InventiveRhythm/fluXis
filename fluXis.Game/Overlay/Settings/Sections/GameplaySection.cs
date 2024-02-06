using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Overlay.Settings.Sections.Gameplay;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GameplaySection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Gamepad;
    public override LocalisableString Title => LocalizationStrings.Settings.Gameplay.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new GameplayGeneralSection(),
            Divider,
            new GameplayScrollSpeedSection(),
            Divider,
            new GameplayMapSection(),
            Divider,
            new GameplayBackgroundSection(),
            Divider,
            new GameplayEffectsSection(),
            Divider,
            new GameplayHudSection()
        });
    }
}
