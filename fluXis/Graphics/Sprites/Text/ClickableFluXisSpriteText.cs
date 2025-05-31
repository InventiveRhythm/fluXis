using System;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.Sprites.Text;

public partial class ClickableFluXisSpriteText : FluXisSpriteText
{
    public Action Action;

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        return Action != null;
    }
}
