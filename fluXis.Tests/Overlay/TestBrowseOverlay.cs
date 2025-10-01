using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Requests.MapSets;
using fluXis.Overlay.Browse;
using fluXis.Tests.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Tests.Overlay;

public partial class TestBrowseOverlay : FluXisTestScene
{
    protected override bool UseTestAPI => true;

    [BackgroundDependencyLoader]
    private void load()
    {
        TestAPI.HandleRequest = req =>
        {
            switch (req)
            {
                case MapSetsRequest mapsets:
                    mapsets.TriggerSuccess(new APIResponse<List<APIMapSet>>(200, "", Enumerable.Repeat(0, 48).Select(_ => TestMapCard.CreateDummy()).ToList()));
                    break;
            }
        };

        CreateDummyBeatSync();

        var overlay = new BrowseOverlay();
        Add(overlay);

        Add(new Box
        {
            RelativeSizeAxes = Axes.X,
            Height = 50,
            Colour = Theme.Background2
        });

        AddStep("Show", overlay.Show);
        AddStep("Hide", overlay.Hide);
    }
}
