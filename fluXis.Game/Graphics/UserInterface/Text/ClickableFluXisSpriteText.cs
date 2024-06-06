using System;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.UserInterface.Text;

public partial class ClickableFluXisSpriteText : FluXisSpriteText
{
    public Action Action;

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        return Action != null;
    }
}
