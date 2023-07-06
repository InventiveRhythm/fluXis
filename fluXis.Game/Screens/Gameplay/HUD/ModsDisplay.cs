using fluXis.Game.Mods.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class ModsDisplay : GameplayHUDElement
{
    private ModList modsContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 60, Right = 10 };

        Child = modsContainer = new ModList { Mods = Screen.Mods };
    }

    protected override void Update()
    {
        if (Screen.Mods.Count == modsContainer.Count) return;

        modsContainer.ReloadList();
    }
}
