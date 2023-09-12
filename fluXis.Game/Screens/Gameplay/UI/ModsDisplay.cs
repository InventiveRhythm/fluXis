using fluXis.Game.Mods.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class ModsDisplay : Container
{
    public GameplayScreen Screen { get; set; }

    private ModList modsContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 80, Right = 20 };

        Child = modsContainer = new ModList { Mods = Screen.Mods };
    }

    protected override void Update()
    {
        if (Screen.Mods.Count == modsContainer.Count) return;

        modsContainer.ReloadList();
    }
}
