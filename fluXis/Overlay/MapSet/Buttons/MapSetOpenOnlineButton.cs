using fluXis.Graphics.Sprites.Icons;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.Fluxel;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;

namespace fluXis.Overlay.MapSet.Buttons;

public partial class MapSetOpenOnlineButton : MapSetButton
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    private Bindable<APIMap> mapBind;

    private APIMapSet set { get; }

    public MapSetOpenOnlineButton(APIMapSet set, Bindable<APIMap> mapBind)
        : base(FontAwesome6.Solid.ShareNodes, () => { })
    {
        this.set = set;
        this.mapBind = mapBind;
    }
    
    protected override bool OnClick(ClickEvent e)
    {
        game?.OpenLink($"{api.Endpoint.WebsiteRootUrl}/set/{set.ID}#{mapBind.Value.ID}");
        return base.OnClick(e);
    }
}
