using System.Collections.Generic;
using fluXis.Shared.Components.Groups;
using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Models.Groups;

public class APIGroup : IAPIGroup
{
    public string ID { get; init; } = "";
    public string Name { get; set; } = "";
    public string Tag { get; set; } = "";
    public string Color { get; set; } = "#ffffff";
    public IEnumerable<APIUserShort> Members { get; } = null!;
}
