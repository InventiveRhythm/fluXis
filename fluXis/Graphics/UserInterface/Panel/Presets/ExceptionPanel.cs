using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Panel.Types;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class ExceptionPanel : SingleButtonPanel
{
    public ExceptionPanel(Exception ex)
        : base(Phosphor.Bold.Bomb, "An unknown error occurred!", ex.Message)
    {
    }
}
