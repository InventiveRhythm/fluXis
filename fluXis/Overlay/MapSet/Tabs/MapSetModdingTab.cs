using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Maps.Modding;
using fluXis.Online.API.Requests.MapSets.Favorite;
using fluXis.Online.Fluxel;
using fluXis.Overlay.MapSet.UI.Modding;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.MapSet.Tabs;

public partial class MapSetModdingTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.PenRuler;
    public override string Title => "Modding";

    [Resolved]
    private IAPIClient api { get; set; }

    private readonly APIMapSet set;

    public MapSetModdingTab(APIMapSet set)
    {
        this.set = set;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var req = new MapSetModdingRequest(set.ID);
        req.Success += res =>
        {
            InternalChild = new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(0, 16),
                ChildrenEnumerable = res.Data.Select(createModdingAction)
            };
        };
        api.PerformRequestAsync(req);
    }

    private Drawable createModdingAction(APIModdingAction action)
    {
        switch (action.Type)
        {
            case APIModdingActionType.Note:
            case APIModdingActionType.Approve:
            case APIModdingActionType.Deny:
                return new ModdingActionComment(action);

            default:
                return new ModdingActionSimple(action);
        }
    }
}
