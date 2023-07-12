using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;

public interface ILobby : IDrawable
{
    bool Joinable { get; }
    bool Selectable { get; }
    bool Selected { get; set; }
    void Join();
}
