using fluXis.Game.Overlay.Mouse;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace fluXis.Game.Tests;

public partial class FluXisTestBrowser : FluXisGameBase
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        AddRange(new Drawable[]
        {
            new TestBrowser("fluXis"),
            new GlobalCursorOverlay()
        });
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        host.Window.CursorState |= CursorState.Hidden;
    }
}
