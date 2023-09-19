using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Maps;
using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Graphics;

public partial class TestMapCard : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(Fluxel fluxel)
    {
        var req = fluxel.CreateAPIRequest("/mapset/4");
        req.Perform();

        var json = req.GetResponseString();
        var res = JsonConvert.DeserializeObject<APIResponse<APIMapSet>>(json);

        Add(new MapCard(res.Data)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });
    }
}
