using System.Collections.Generic;
using fluXis.Shared.Components.Other;

namespace fluXis.Game.Online.API.Requests;

public class MenuUpdatesRequest : APIRequest<List<MenuUpdate>>
{
    protected override string Path => "/updates.json";
    protected override string RootUrl => APIClient.Endpoint.AssetUrl;
}
