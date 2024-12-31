using System;
using fluXis.Graphics.UserInterface.Panel.Types;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class ExceptionPanel : SingleButtonPanel
{
    public ExceptionPanel(Exception ex)
        : base(FontAwesome.Solid.Bomb, "An unknown error occurred!", ex.Message)
    {
    }
}
