using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Graphics;

public partial class TestMapCard : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(IAPIClient api)
    {
        var req = new MapSetRequest(4);
        api.PerformRequest(req);

        Add(new MapCard(req.Response.Data)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });
    }
}
