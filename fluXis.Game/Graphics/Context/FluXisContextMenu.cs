using fluXis.Game.Graphics.Menu;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;

namespace fluXis.Game.Graphics.Context;

public partial class FluXisContextMenu : FluXisMenu
{
    public FluXisContextMenu()
        : base(Direction.Vertical)
    {
        MaskingContainer.EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Colour4.Black.Opacity(0.1f),
            Radius = 4
        };
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new FluXisContextMenu();
}
