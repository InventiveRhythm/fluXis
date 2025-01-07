using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Panel.Types;

namespace fluXis.Screens.Multiplayer;

public partial class DisconnectedPanel : SingleButtonPanel
{
    public DisconnectedPanel(Action closeAction)
        : base(FontAwesome6.Solid.PlugCircleXMark, "Connection Lost", "The connection to the server was interrupted.", action: closeAction)
    {
    }
}
