using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;

public partial class EmptyLobbySlot : Container, ILobby
{
    public bool Joinable => false;
    public bool Selectable => false;
    public bool Selected { get; set; }
    public void Join() => throw new NotImplementedException("You can't join an empty lobby slot!");

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
