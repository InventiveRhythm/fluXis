using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Panel.Types;

namespace fluXis.Screens.Multiplayer;

public partial class DisconnectedPanel : SingleButtonPanel
{
    public DisconnectedPanel(Action closeAction)
        : base(Phosphor.Bold.Plugs, "Connection Lost", "The connection to the server was interrupted.", action: closeAction)
    {
    }
}
