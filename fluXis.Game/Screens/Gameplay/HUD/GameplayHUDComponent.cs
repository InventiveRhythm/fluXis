using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class GameplayHUDComponent : Container
{
    [Resolved]
    protected GameplayScreen Screen { get; private set; }

    public HUDComponentSettings Settings { get; set; } = new();
}
