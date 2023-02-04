using Realms;

namespace fluXis.Game.Database;

public class RealmFile : RealmObject
{
    public string Hash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string GetPath()
    {
        return Hash[..1] + "/" + Hash[..2] + "/" + Hash;
    }

    public override string ToString()
    {
        return $"{Hash} - {Name}";
    }
}
