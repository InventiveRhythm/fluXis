using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Graphics;

public partial class TestMapCard : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(FluxelClient fluxel)
    {
        var req = fluxel.CreateAPIRequest("/mapset/4");
        req.Perform();

        var json = req.GetResponseString();
        var res = json.Deserialize<APIResponse<APIMapSet>>();

        Add(new MapCard(res.Data)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });
    }
}
