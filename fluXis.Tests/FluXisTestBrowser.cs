using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace fluXis.Tests;

public partial class FluXisTestBrowser : FluXisGameBase
{
    private readonly string assembly;

    public FluXisTestBrowser(string assembly)
    {
        this.assembly = assembly;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        AddRange(new Drawable[]
        {
            new TestBrowser(assembly)
        });
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        host.Window.CursorState = CursorState.Hidden;
    }
}
