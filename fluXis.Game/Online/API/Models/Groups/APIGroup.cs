using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Users;

namespace fluXis.Game.Online.API.Models.Groups;

public class APIGroup : IAPIGroup
{
    public string ID { get; init; } = "";
    public string Name { get; set; } = "";
    public string Tag { get; set; } = "";
    public string Color { get; set; } = "#ffffff";
    public IEnumerable<APIUser> Members { get; } = null!;
}
