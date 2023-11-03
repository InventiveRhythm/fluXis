using fluXis.Game.Mods.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class ModsDisplay : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    private ModList modsContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 80, Right = 20 };

        Child = modsContainer = new ModList { Mods = screen.Mods };
    }

    protected override void Update()
    {
        if (screen.Mods.Count == modsContainer.Count) return;

        modsContainer.ReloadList();
    }
}
