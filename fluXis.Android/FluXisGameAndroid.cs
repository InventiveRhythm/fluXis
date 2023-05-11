using fluXis.Game;
using osu.Framework.Platform;

namespace fluXis.Android;

public partial class FluXisGameAndroid : FluXisGame
{
    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        ExitAction = host.Exit;
    }
}
