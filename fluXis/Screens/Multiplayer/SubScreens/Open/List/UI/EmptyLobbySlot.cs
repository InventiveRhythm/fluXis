using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.List.UI;

public partial class EmptyLobbySlot : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 650;
        Height = 90;
        CornerRadius = 10;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Black,
            Alpha = .5f
        };
    }
}
