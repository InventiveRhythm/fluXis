using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Panel;

namespace fluXis.Game.Screens.Multiplayer;

public partial class DisconnectedPanel : SingleButtonPanel
{
    public DisconnectedPanel(Action closeAction)
        : base(FontAwesome6.Solid.PlugCircleXMark, "Connection Lost", "The connection to the server was interrupted.", action: closeAction)
    {
    }
}
