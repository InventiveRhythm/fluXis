using System.Collections.Generic;
using fluXis.Online.API.Models.Other;

namespace fluXis.Online.API.Requests;

public class MenuUpdatesRequest : APIRequest<List<MenuUpdate>>
{
    protected override string Path => "/updates.json";
    protected override string RootUrl => APIClient.Endpoint.AssetUrl;
}
