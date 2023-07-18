using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics;

public partial class FullInputBlockingContainer : Container
{
    protected override bool Handle(UIEvent e)
    {
        base.Handle(e);
        return true;
    }
}
