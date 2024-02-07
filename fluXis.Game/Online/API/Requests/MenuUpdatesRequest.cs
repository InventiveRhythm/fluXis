using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Other;

namespace fluXis.Game.Online.API.Requests;

public class MenuUpdatesRequest : APIRequest<List<MenuUpdate>>
{
    protected override string Path => "/updates.json";
    protected override string RootUrl => Config.AssetUrl;
}
