using System;
using fluXis.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.UserInterface.Text;

public partial class ClickableFluXisSpriteText : FluXisSpriteText
{
    public Action Action;

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        return Action != null;
    }
}
